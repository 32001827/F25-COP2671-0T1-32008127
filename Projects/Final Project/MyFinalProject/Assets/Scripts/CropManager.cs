using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CropManager : SingletonMonoBehaviour<CropManager>
{

    [Header("References")]
    [SerializeField] 
    private Tilemap farmingTilemap;

    [SerializeField]
    private GameObject harvestReadyEffectPrefab;

    public GameObject HarvestReadyEffectPrefab => harvestReadyEffectPrefab;

    [Header("Crop Data")]
    private CropBlock[,] cropGrid;

    private List<CropBlock> plantedCrops = new List<CropBlock>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (farmingTilemap != null)
        {
            CreateGridUsingTilemap(farmingTilemap);
        }
    }

    private void OnEnable()
    {
        TimeManager.OnGameHourPassed += HandleGameHourPassed;
    }

    private void OnDisable()
    {
        TimeManager.OnGameHourPassed -= HandleGameHourPassed;
    }

    private void HandleGameHourPassed(int hour)
    {
        foreach (CropBlock cropB in plantedCrops)
        {
            cropB.Grow(60);
        }
    }

    public void CreateGridUsingTilemap(Tilemap tilemap)
    {
        tilemap.CompressBounds();
        BoundsInt bounds = tilemap.cellBounds;

        cropGrid = new CropBlock[bounds.size.x, bounds.size.y];

        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                Vector2Int location = new Vector2Int(bounds.xMin + x, bounds.yMin + y);
                CreateGridBlock(location);
            }
        }
    }

    public void CreateGridBlock(Vector2Int location)
    {
        BoundsInt bounds = farmingTilemap.cellBounds;
        int gridX = location.x - bounds.xMin;
        int gridY = location.y - bounds.yMin;

        if (gridX >= 0 && gridX < cropGrid.GetLength(0) &&
            gridY >= 0 && gridY < cropGrid.GetLength(1))
        {
            CropBlock gridBlock = new CropBlock(location, farmingTilemap);
            cropGrid[gridX, gridY] = gridBlock;
        }
        else
        {
            Debug.LogError("Location is out of bounds of the crop grid.");
        }
    }

    public void AddToPlantedCrops(CropBlock cropBlock)
    {
        if (!plantedCrops.Contains(cropBlock))
        {
            plantedCrops.Add(cropBlock);
        }
    }

    public void RemoveFromPlantedCrops(CropBlock cropBlock)
    {
        if (plantedCrops.Contains(cropBlock))
        {
            plantedCrops.Remove(cropBlock);
        }

        BoundsInt bounds = farmingTilemap.cellBounds;
        int gridX = cropBlock.location.x - bounds.xMin;
        int gridY = cropBlock.location.y - bounds.yMin;

        if (gridX >= 0 && gridX < cropGrid.GetLength(0) &&
            gridY >= 0 && gridY < cropGrid.GetLength(1))
        {
            cropGrid[gridX, gridY] = null;
        }
        else
        {
            Debug.LogError("Location is out of bounds of the crop grid.");
        }
    }

    public CropBlock GetBlockAt(Vector2Int location)
    {
        BoundsInt bounds = farmingTilemap.cellBounds;
        int gridX = location.x - bounds.xMin;
        int gridY = location.y - bounds.yMin;

        if (gridX >= 0 && gridX < cropGrid.GetLength(0) &&
            gridY >= 0 && gridY < cropGrid.GetLength(1))
        {
            return cropGrid[gridX, gridY];
        }
        else
        {
            Debug.LogError("Location is out of bounds of the crop grid.");
            return null;
        }
    }
}
