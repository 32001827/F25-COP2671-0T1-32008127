using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Manages the grid of crops, including soil states, growth updates, and visual tiles.
/// </summary>
public class CropManager : SingletonMonoBehaviour<CropManager>
{
    [Header("References")]
    [SerializeField] private Tilemap farmingTilemap;
    [SerializeField] private Tilemap soilTilemap;

    [Header("Settings")]
    [Tooltip("Time in minutes before tilled soil reverts to default.")]
    [SerializeField] private float minutesToUntill = 720f;

    [Header("Visuals")]
    [SerializeField] private GameObject harvestReadyEffectPrefab;
    [SerializeField] private TileBase tilledTile;
    [SerializeField] private TileBase wateredTile;
    [Tooltip("The tile to use when soil is NOT tilled. Leave empty if you want it to be clear/invisible.")]
    [SerializeField] private TileBase defaultTile;

    [Header("Database")]
    [Tooltip("Drag ALL SeedPacket assets here so they can be loaded by name.")]
    [SerializeField] private List<SeedPacket> allSeedPackets;

    /// <summary>
    /// Gets the particle effect prefab for harvest-ready crops.
    /// </summary>
    public GameObject HarvestReadyEffectPrefab => harvestReadyEffectPrefab;

    private CropBlock[,] cropGrid;
    private List<CropBlock> activeBlocks = new List<CropBlock>();
    private BoundsInt gridBounds;

    private void Start()
    {
        if (soilTilemap != null && cropGrid == null)
        {
            CreateGridUsingTilemap(soilTilemap);
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

    private void HandleGameMinutePassed(int minute)
    {
        for (int i = activeBlocks.Count - 1; i >= 0; i--)
        {
            bool keepActive = activeBlocks[i].GameUpdate(1, minutesToUntill);

            if (!keepActive)
            {
                activeBlocks.RemoveAt(i);
            }
        }
    }

    private void HandleGameHourPassed(int hour) { }

    /// <summary>
    /// Updates the visual tile on the soil map based on tilled/watered state.
    /// </summary>
    /// <param name="gridLocation">The grid coordinate to update.</param>
    /// <param name="isTilled">Whether the soil is tilled.</param>
    /// <param name="isWatered">Whether the soil is watered.</param>
    public void UpdateSoilSprite(Vector2Int gridLocation, bool isTilled, bool isWatered)
    {
        if (soilTilemap == null) return;

        TileBase tileToSet = defaultTile;

        if (isWatered) tileToSet = wateredTile;
        else if (isTilled) tileToSet = tilledTile;

        soilTilemap.SetTile((Vector3Int)gridLocation, tileToSet);
    }

    /// <summary>
    /// Initializes the grid data structure based on the bounds of the provided tilemap.
    /// </summary>
    /// <param name="tilemap">The tilemap defining the grid bounds.</param>
    public void CreateGridUsingTilemap(Tilemap tilemap)
    {
        tilemap.CompressBounds();
        gridBounds = tilemap.cellBounds;

        cropGrid = new CropBlock[gridBounds.size.x, gridBounds.size.y];

        for (int x = 0; x < gridBounds.size.x; x++)
        {
            for (int y = 0; y < gridBounds.size.y; y++)
            {
                Vector2Int location = new Vector2Int(gridBounds.xMin + x, gridBounds.yMin + y);
                CreateGridBlock(location);
            }
        }
    }

    /// <summary>
    /// Creates a new CropBlock at the specified location.
    /// </summary>
    /// <param name="location">The grid coordinate for the block.</param>
    public void CreateGridBlock(Vector2Int location)
    {
        int gridX = location.x - gridBounds.xMin;
        int gridY = location.y - gridBounds.yMin;

        if (gridX >= 0 && gridX < cropGrid.GetLength(0) && gridY >= 0 && gridY < cropGrid.GetLength(1))
        {
            CropBlock gridBlock = new CropBlock(location, farmingTilemap);
            cropGrid[gridX, gridY] = gridBlock;
        }
    }

    /// <summary>
    /// Registers a block to receive updates (growth or soil decay).
    /// </summary>
    /// <param name="cropBlock">The block to add.</param>
    public void RegisterActiveBlock(CropBlock cropBlock)
    {
        if (!activeBlocks.Contains(cropBlock)) activeBlocks.Add(cropBlock);
    }

    /// <summary>
    /// Adds a crop block to the active update list.
    /// </summary>
    /// <param name="cropBlock">The block to add.</param>
    public void AddToPlantedCrops(CropBlock cropBlock)
    {
        RegisterActiveBlock(cropBlock);
    }

    /// <summary>
    /// Removes a crop from the list of active crops.
    /// </summary>
    /// <param name="cropBlock">The block to remove.</param>
    public void RemoveFromPlantedCrops(CropBlock cropBlock)
    {
        if (cropBlock.IsTilled) return;

        if (activeBlocks.Contains(cropBlock)) activeBlocks.Remove(cropBlock);
    }

    /// <summary>
    /// Retrieves the CropBlock at the specified grid location.
    /// </summary>
    /// <param name="location">The grid coordinate.</param>
    /// <returns>The CropBlock object, or null if out of bounds.</returns>
    public CropBlock GetBlockAt(Vector2Int location)
    {
        int gridX = location.x - gridBounds.xMin;
        int gridY = location.y - gridBounds.yMin;

        if (gridX >= 0 && gridX < cropGrid.GetLength(0) && gridY >= 0 && gridY < cropGrid.GetLength(1))
        {
            return cropGrid[gridX, gridY];
        }
        return null;
    }

    /// <summary>
    /// Restores crop data from a save file.
    /// </summary>
    /// <param name="loadedCrops">The list of saved crop data.</param>
    public void LoadCrops(List<SaveData.CropData> loadedCrops)
    {
        farmingTilemap.ClearAllTiles();
        activeBlocks.Clear();

        CreateGridUsingTilemap(soilTilemap);

        foreach (var data in loadedCrops)
        {
            CropBlock block = GetBlockAt(data.Location);
            if (block != null)
            {
                if (data.IsTilled)
                {
                    block.TillSoil();
                    block.SetTilledTimer(data.TilledTimer);
                }

                if (data.IsWatered) block.WaterSoil();

                if (!string.IsNullOrEmpty(data.SeedName))
                {
                    SeedPacket seed = allSeedPackets.Find(s => s.CropName == data.SeedName);
                    if (seed != null)
                    {
                        block.ForceLoadCrop(seed, data.GrowthStage, data.GrowthTimer);
                    }
                }

                if (block.IsTilled || block.SeedPacket != null)
                {
                    RegisterActiveBlock(block);
                }
            }
        }

        if (cropGrid != null)
        {
            foreach (var block in cropGrid)
            {
                if (block != null)
                {
                    UpdateSoilSprite(block.Location, block.IsTilled, block.IsWatered);
                }
            }
        }
    }

    /// <summary>
    /// Retrieves all active CropBlocks for saving.
    /// </summary>
    /// <returns>A list of modified CropBlocks.</returns>
    public List<CropBlock> GetAllCrops()
    {
        List<CropBlock> cropsToSave = new List<CropBlock>();
        if (cropGrid == null) return cropsToSave;

        foreach (var block in cropGrid)
        {
            if (block != null && (block.IsTilled || block.SeedPacket != null))
            {
                cropsToSave.Add(block);
            }
        }
        return cropsToSave;
    }
}