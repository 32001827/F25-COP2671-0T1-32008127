using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class CropBlock
{
    public SeedPacket seedPacket;
    public int currentGrowthStage;
    public float growthTimer;

    public Vector2Int location;
    public bool isWatered;
    public bool isTilled;

    private Tilemap farmingTilemap;
    private Tile tempTile;

    private GameObject harvestEffectInstance;

    public CropBlock(Vector2Int loc, Tilemap map)
    {
        this.location = loc;
        this.farmingTilemap = map;
        this.tempTile = ScriptableObject.CreateInstance<Tile>();

        isTilled = false;
        isWatered = false;
        currentGrowthStage = 0;
        growthTimer = 0f;
        seedPacket = null;
    }

    public bool CanPlant()
    {
        return isTilled && isWatered;
    }

    public void TillSoil()
    {
        isTilled = true;
    }

    public void WaterSoil()
    {
        if (isTilled)
        {
            isWatered = true;
        }
    }

    public void PlantSeed(SeedPacket seed)
    {
        if (!CanPlant())
        {
            Debug.LogWarning("Soil must be tilled before planting seeds.");
            return;
        }

        this.seedPacket = seed;
        this.currentGrowthStage = 0;
        this.growthTimer = 0f;

        UpdateGrowthSprite();
    }

    public void HarvestPlants()
    {
        StopHarvestreadyEffect();

        if (seedPacket == null || !IsFullyGrown())
        {
            Debug.LogWarning("No crop planted to harvest.");
            return;
        }

        Vector3 worldPosistion = farmingTilemap.CellToWorld((Vector3Int)location);
        worldPosistion += new Vector3(0.5f, 0.5f, 0f);

        Object.Instantiate(seedPacket.harvestablePrefab, worldPosistion, Quaternion.identity);

        farmingTilemap.SetTile((Vector3Int)location, null);

        seedPacket = null;
        currentGrowthStage = 0;
        growthTimer = 0f;
        isWatered = false;
    }

    public void Grow(float gameMinutes)
    {
        if (seedPacket == null || IsFullyGrown() || !isWatered)
        {
            return;
        }

        growthTimer += gameMinutes;

        int numGrowthStages = seedPacket.growthSprites.Length;

        float timePerStage = 0f;

        if (numGrowthStages > 1)
        {
            timePerStage = (float)seedPacket.growthTimeInMinutes / (numGrowthStages - 1);
        }
        
        int newStage = Mathf.FloorToInt(growthTimer / timePerStage);
        if (newStage > currentGrowthStage)
        {
            currentGrowthStage = newStage;

            currentGrowthStage = Mathf.Min(currentGrowthStage, numGrowthStages - 1);

            UpdateGrowthSprite();

            if (IsFullyGrown())
            {
                StartHarvestreadyEffect();
            }
        }
    }

    private void StartHarvestreadyEffect()
    {
        if (harvestEffectInstance != null) return;

        GameObject prefab = CropManager.Instance.HarvestReadyEffectPrefab;
        
        if (prefab == null) return;

        Vector3 worldPos = farmingTilemap.CellToWorld((Vector3Int)location) + new Vector3(0.5f, 0.5f, 0);

        harvestEffectInstance = Object.Instantiate(prefab, worldPos, Quaternion.identity);
    }

    private void StopHarvestReadyEffect()
    {
        if (harvestEffectInstance != null)
        {
            Object.Destroy(harvestEffectInstance);
            harvestEffectInstance = null;
        }
    }

    public bool IsFullyGrown()
    {
        if (seedPacket == null)
        {
            return false;
        }
        return currentGrowthStage >= seedPacket.growthSprites.Length - 1;
    }

    private void UpdateGrowthSprite()
    {
        if (seedPacket == null || seedPacket.growthSprites.Length == 0)
        {
            return;
        }

        tempTile.sprite = seedPacket.growthSprites[currentGrowthStage];

        farmingTilemap.SetTile((Vector3Int)location, tempTile);
    }
}
