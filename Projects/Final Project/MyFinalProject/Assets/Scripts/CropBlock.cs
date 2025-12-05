using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Represents the data and state of a single crop tile in the game.
/// </summary>
[System.Serializable]
public class CropBlock
{
    public SeedPacket SeedPacket { get; private set; }
    public int CurrentGrowthStage { get; private set; }
    public float GrowthTimer { get; private set; }
    public Vector2Int Location { get; }
    public bool IsWatered { get; private set; }
    public bool IsTilled { get; private set; }
    public float TilledTimer { get; private set; }

    private Tilemap farmingTilemap;
    private GameObject harvestReadyEffectInstance;

    /// <summary>
    /// Initializes a new instance of the CropBlock class.
    /// </summary>
    /// <param name="loc">The grid location of the block.</param>
    /// <param name="map">The tilemap used for rendering crops.</param>
    public CropBlock(Vector2Int loc, Tilemap map)
    {
        Location = loc;
        farmingTilemap = map;

        IsTilled = false;
        IsWatered = false;
        CurrentGrowthStage = 0;
        GrowthTimer = 0f;
        TilledTimer = 0f;
        SeedPacket = null;
    }

    /// <summary>
    /// Forces a crop into this block from save data, bypassing validation.
    /// </summary>
    /// <param name="seed">The seed packet to load.</param>
    /// <param name="stage">The current growth stage.</param>
    /// <param name="timer">The elapsed growth time.</param>
    public void ForceLoadCrop(SeedPacket seed, int stage, float timer)
    {
        SeedPacket = seed;
        CurrentGrowthStage = stage;
        GrowthTimer = timer;
        TilledTimer = 0f;

        UpdateGrowthSprite();
    }

    /// <summary>
    /// Sets the soil decay timer manually (used for loading saved data).
    /// </summary>
    /// <param name="timer">The elapsed time since tilled.</param>
    public void SetTilledTimer(float timer)
    {
        TilledTimer = timer;
    }

    /// <summary>
    /// Checks if the soil is ready AND empty for a new seed.
    /// </summary>
    /// <returns>True if planting is allowed; otherwise, false.</returns>
    public bool CanPlant()
    {
        if (SeedPacket != null)
        {
            return false;
        }

        return IsTilled && IsWatered;
    }

    /// <summary>
    /// Sets the soil state to Tilled and updates the visual.
    /// </summary>
    public void TillSoil()
    {
        if (IsTilled) return;

        IsTilled = true;
        TilledTimer = 0f;
        CropManager.Instance.UpdateSoilSprite(Location, IsTilled, IsWatered);

        CropManager.Instance.RegisterActiveBlock(this);
    }

    /// <summary>
    /// Reverts the soil to its default state (untilled, unwatered).
    /// </summary>
    public void UntillSoil()
    {
        IsTilled = false;
        IsWatered = false;
        TilledTimer = 0f;

        CropManager.Instance.UpdateSoilSprite(Location, IsTilled, IsWatered);
    }

    /// <summary>
    /// Sets the soil state to Watered (if tilled) and updates the visual.
    /// </summary>
    public void WaterSoil()
    {
        if (!IsTilled) return;

        if (IsWatered) return;

        IsWatered = true;
        TilledTimer = 0f;
        CropManager.Instance.UpdateSoilSprite(Location, IsTilled, IsWatered);
    }

    /// <summary>
    /// Plants the given seed in this block if the soil is ready.
    /// </summary>
    /// <param name="seed">The seed packet to plant.</param>
    public void PlantSeed(SeedPacket seed)
    {
        if (!CanPlant())
        {
            Debug.LogWarning("Cannot plant: Soil is not ready or crop already exists.");
            return;
        }

        SeedPacket = seed;
        CurrentGrowthStage = 0;
        GrowthTimer = 0f;
        TilledTimer = 0f;

        UpdateGrowthSprite();

        CropManager.Instance.RegisterActiveBlock(this);
    }

    /// <summary>
    /// Harvests the plant, resets water state, and returns the ItemData.
    /// </summary>
    /// <returns>The item yielded by the crop, or null if not ready.</returns>
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
        TilledTimer = 0f;

        CropManager.Instance.UpdateSoilSprite(Location, IsTilled, IsWatered);

        return itemToReturn;
    }

    /// <summary>
    /// Updates the block's state. Handles both crop growth and soil decay.
    /// </summary>
    /// <param name="gameMinutes">The number of game minutes to advance.</param>
    /// <param name="minutesToUntill">The threshold in minutes before soil untils itself.</param>
    /// <returns>True if the block is still active (has seed OR is tilled); false if it became inactive.</returns>
    public bool GameUpdate(float gameMinutes, float minutesToUntill)
    {
        if (SeedPacket != null)
        {
            Grow(gameMinutes);
            return true;
        }

        if (IsTilled)
        {
            TilledTimer += gameMinutes;
            if (TilledTimer >= minutesToUntill)
            {
                UntillSoil();
                return false;
            }
            return true;
        }

        return false;
    }

    /// <summary>
    /// Advances the growth of the plant based on elapsed time.
    /// </summary>
    private void Grow(float gameMinutes)
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

    private void StartHarvestReadyEffect()
    {
        if (harvestReadyEffectInstance != null) return;

        GameObject prefab = CropManager.Instance.HarvestReadyEffectPrefab;

        if (prefab == null) return;

        Vector3 worldPos = farmingTilemap.CellToWorld((Vector3Int)Location) + new Vector3(0.5f, 0.5f, 0);

        harvestReadyEffectInstance = Object.Instantiate(prefab, worldPos, Quaternion.identity);
    }

    private void StopHarvestReadyEffect()
    {
        if (harvestReadyEffectInstance != null)
        {
            Object.Destroy(harvestReadyEffectInstance);
            harvestReadyEffectInstance = null;
        }
    }

    /// <summary>
    /// Checks if the crop has reached its final growth stage.
    /// </summary>
    /// <returns>True if fully grown; otherwise, false.</returns>
    public bool IsFullyGrown()
    {
        if (SeedPacket == null)
        {
            return false;
        }
        return CurrentGrowthStage >= SeedPacket.GrowthSprites.Length - 1;
    }

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