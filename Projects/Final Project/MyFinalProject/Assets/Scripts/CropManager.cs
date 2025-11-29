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

    /// <summary>
    /// The particle effect prefab to spawn when a crop is ready to harvest.
    /// </summary>
    public GameObject HarvestReadyEffectPrefab => harvestReadyEffectPrefab;

    [Header("Crop Data")]
    private CropBlock[,] cropGrid;
    private List<CropBlock> plantedCrops = new List<CropBlock>();

    private void Start()
    {
        if (farmingTilemap != null)
        {
            CreateGridUsingTilemap(farmingTilemap);
        }
    }

    private void OnEnable()
    {
        TimeManager.OnGameHourPassed += HandleGameHourPassed;
        TimeManager.OnGameMinutePassed += HandleGameMinutePassed;
    }

    private void OnDisable()
    {
        TimeManager.OnGameHourPassed -= HandleGameHourPassed;
        TimeManager.OnGameMinutePassed -= HandleGameMinutePassed;
    }

    /// <summary>
    /// Called by the TimeManager every game minute.
    /// </summary>
    /// <param name="minute">The current minute.</param>
    private void HandleGameMinutePassed(int minute)
    {
        for (int i = plantedCrops.Count - 1; i >= 0; i--)
        {
            plantedCrops[i].Grow(1);
        }
    }

    /// <summary>
    /// Called by the TimeManager every game hour.
    /// </summary>
    /// <param name="hour">The new hour.</param>
    private void HandleGameHourPassed(int hour)
    {
        // Reserved for future hourly logic
    }

    /// <summary>
    /// Initializes the grid of CropBlocks based on the tilemap's bounds.
    /// </summary>
    /// <param name="tilemap">The farming tilemap.</param>
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

    /// <summary>
    /// Creates a new CropBlock and adds it to the grid array.
    /// </summary>
    /// <param name="location">The cell location for the new block.</param>
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
            Debug.LogError($"CreateGridBlock: Location {location} is out of bounds.");
        }
    }

    /// <summary>
    /// Adds a CropBlock to the list of actively growing crops.
    /// </summary>
    /// <param name="cropBlock">The block to add.</param>
    public void AddToPlantedCrops(CropBlock cropBlock)
    {
        if (!plantedCrops.Contains(cropBlock))
        {
            plantedCrops.Add(cropBlock);
        }
    }

    /// <summary>
    /// Removes a CropBlock from the active growth list and the grid lookup.
    /// </summary>
    /// <param name="cropBlock">The block to remove.</param>
    public void RemoveFromPlantedCrops(CropBlock cropBlock)
    {
        if (plantedCrops.Contains(cropBlock))
        {
            plantedCrops.Remove(cropBlock);
        }

        BoundsInt bounds = farmingTilemap.cellBounds;
        int gridX = cropBlock.Location.x - bounds.xMin;
        int gridY = cropBlock.Location.y - bounds.yMin;

        if (gridX >= 0 && gridX < cropGrid.GetLength(0) &&
            gridY >= 0 && gridY < cropGrid.GetLength(1))
        {
            cropGrid[gridX, gridY] = new CropBlock(cropBlock.Location, farmingTilemap);
        }
    }

    /// <summary>
    /// Gets the CropBlock data for a specific cell location.
    /// </summary>
    /// <param name="location">The cell location to check.</param>
    /// <returns>The CropBlock at that location, or null if out of bounds.</returns>
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
            return null;
        }
    }
}