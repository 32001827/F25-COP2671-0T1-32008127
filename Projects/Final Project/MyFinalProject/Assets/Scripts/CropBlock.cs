using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class CropBlock
{
    public SeedPacket SeedPacket { get; private set; }
    public int CurrentGrowthStage { get; private set; }
    public float GrowthTimer { get; private set; }
    public Vector2Int Location { get; }
    public bool IsWatered { get; private set; }
    public bool IsTilled { get; private set; }

    private Tilemap farmingTilemap;
    private GameObject harvestReadyEffectInstance;

    public CropBlock(Vector2Int loc, Tilemap map)
    {
        this.Location = loc;
        this.farmingTilemap = map;

        IsTilled = false;
        IsWatered = false;
        CurrentGrowthStage = 0;
        GrowthTimer = 0f;
        SeedPacket = null;
    }

    /// <summary>
    /// Checks if the soil is in a state to allow planting.
    /// </summary>
    /// <returns>True if tilled and watered, false otherwise.</returns>
    public bool CanPlant()
    {
        return IsTilled && IsWatered;
    }

    /// <summary>
    /// Sets the soil state to Tilled.
    /// </summary>
    public void TillSoil()
    {
        IsTilled = true;
    }

    /// <summary>
    /// Sets the soil state to Watered, if it is already tilled.
    /// </summary>
    public void WaterSoil()
    {
        if (IsTilled)
        {
            IsWatered = true;
        }
    }

    /// <summary>
    /// Plants the given seed in this block if the soil is ready.
    /// </summary>
    /// <param name="seed">The SeedPacket ScriptableObject to plant.</param>
    public void PlantSeed(SeedPacket seed)
    {
        if (!CanPlant())
        {
            Debug.LogWarning("Soil must be tilled AND watered before planting seeds.");
            return;
        }

        this.SeedPacket = seed;
        this.CurrentGrowthStage = 0;
        this.GrowthTimer = 0f;

        UpdateGrowthSprite();
    }

    /// <summary>
    /// Harvests the plant and returns the ItemData to be added to inventory.
    /// Returns null if nothing was harvested.
    /// </summary>
    public ItemData HarvestPlants()
    {
        StopHarvestReadyEffect();

        if (SeedPacket == null || !IsFullyGrown())
        {
            Debug.LogWarning("No crop planted to harvest.");
            return null;
        }

        ItemData itemToReturn = SeedPacket.ItemYield;

        farmingTilemap.SetTile((Vector3Int)Location, null);

        SeedPacket = null;
        CurrentGrowthStage = 0;
        GrowthTimer = 0f;
        IsWatered = false;

        return itemToReturn;
    }

    /// <summary>
    /// Advances the growth of the plant based on elapsed time.
    /// </summary>
    /// <param name="gameMinutes">The number of in-game minutes that have passed.</param>
    public void Grow(float gameMinutes)
    {
        if (SeedPacket == null || IsFullyGrown() || !IsWatered)
        {
            return;
        }

        GrowthTimer += gameMinutes;

        int numGrowthStages = SeedPacket.GrowthSprites.Length;

        float timePerStage = 0f;

        if (numGrowthStages > 1)
        {
            timePerStage = (float)SeedPacket.GrowthTimeInMinutes / (numGrowthStages - 1);
        }

        int newStage = 0;
        if (timePerStage > 0)
        {
            newStage = Mathf.FloorToInt(GrowthTimer / timePerStage);
        }

        if (newStage > CurrentGrowthStage)
        {
            CurrentGrowthStage = newStage;
            CurrentGrowthStage = Mathf.Min(CurrentGrowthStage, numGrowthStages - 1);

            UpdateGrowthSprite();

            if (IsFullyGrown())
            {
                StartHarvestReadyEffect();
            }
        }
    }

    /// <summary>
    /// Spawns the particle effect prefab to indicate the crop is harvestable.
    /// </summary>
    private void StartHarvestReadyEffect()
    {
        if (harvestReadyEffectInstance != null) return;

        GameObject prefab = CropManager.Instance.HarvestReadyEffectPrefab;

        if (prefab == null) return;

        Vector3 worldPos = farmingTilemap.CellToWorld((Vector3Int)Location) + new Vector3(0.5f, 0.5f, 0);

        harvestReadyEffectInstance = Object.Instantiate(prefab, worldPos, Quaternion.identity);
    }

    /// <summary>
    /// Destroys the active harvestable particle effect.
    /// </summary>
    private void StopHarvestReadyEffect()
    {
        if (harvestReadyEffectInstance != null)
        {
            Object.Destroy(harvestReadyEffectInstance);
            harvestReadyEffectInstance = null;
        }
    }

    /// <summary>
    /// Checks if the crop is at its final growth stage.
    /// </summary>
    /// <returns>True if the crop is fully grown, false otherwise.</returns>
    public bool IsFullyGrown()
    {
        if (SeedPacket == null)
        {
            return false;
        }
        return CurrentGrowthStage >= SeedPacket.GrowthSprites.Length - 1;
    }

    /// <summary>
    /// Updates the tilemap sprite to match the crop's current growth stage.
    /// </summary>
    private void UpdateGrowthSprite()
    {
        if (SeedPacket == null || SeedPacket.GrowthSprites.Length == 0)
        {
            return;
        }

        Tile tileToSet = ScriptableObject.CreateInstance<Tile>();
        tileToSet.sprite = SeedPacket.GrowthSprites[CurrentGrowthStage];

        farmingTilemap.SetTile((Vector3Int)Location, tileToSet);
    }
}