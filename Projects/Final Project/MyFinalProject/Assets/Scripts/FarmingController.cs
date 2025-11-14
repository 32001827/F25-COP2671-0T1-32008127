using UnityEngine;
using UnityEngine.Tilemaps;

public class FarmingController : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private CropManager cropManager;

    [SerializeField]
    private Tilemap farmingTilemap;

    private CropBlock selectedBlock;
    private Vector2Int playerGridLocation;

    private void OnEnable()
    {
        ToolBarController.OnHoeToolUsed += HandleHoeTool;
        ToolBarController.OnWaterToolUsed += HandleWaterTool;
        ToolBarController.OnPlantToolUsed += HandlePlantTool;
        ToolBarController.OnHarvestToolUsed += HandleHarvestTool;
    }

    private void OnDisable()
    {
        ToolBarController.OnHoeToolUsed -= HandleHoeTool;
        ToolBarController.OnWaterToolUsed -= HandleWaterTool;
        ToolBarController.OnPlantToolUsed -= HandlePlantTool;
        ToolBarController.OnHarvestToolUsed -= HandleHarvestTool;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3Int playerCell = farmingTilemap.WorldToCell(transform.position);
        playerGridLocation = (Vector2Int)playerCell;

        selectedBlock = cropManager.GetBlockAt(playerGridLocation);
    }
    private void HandleHoeTool()
    {
        if (selectedBlock == null) return;
        selectedBlock.TillSoil(); 
    }

    private void HandleWaterTool()
    {
        if (selectedBlock == null) return;
        selectedBlock.WaterSoil(); 
    }

    private void HandlePlantTool()
    {
        if (selectedBlock == null) return;

        SeedPacket seed = ToolBarController.activeSeedPacket; 
        if (seed != null)
        {
            selectedBlock.PlantSeed(seed); 
            cropManager.AddToPlantedCrops(selectedBlock); 
        }
    }

    private void HandleHarvestTool()
    {
        if (selectedBlock == null) return;
        selectedBlock.HarvestPlants(); 
        cropManager.RemoveFromPlantedCrops(selectedBlock); 
    }

}
