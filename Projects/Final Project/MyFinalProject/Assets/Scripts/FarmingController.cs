using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Handles player input for farming actions and triggers animations/logic.
/// </summary>
public class FarmingController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CropManager cropManager;
    [SerializeField] private Tilemap farmingTilemap;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerController playerController;

    private InventoryManager inventoryManager;
    private CropBlock selectedBlock;
    private Vector2Int playerGridLocation;
    private bool isUsingTool = false;

    private void Start()
    {
        inventoryManager = GetComponent<InventoryManager>();
    }

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

    void Update()
    {
        if (isUsingTool) return;

        Vector3Int playerCell = farmingTilemap.WorldToCell(transform.position);
        playerGridLocation = (Vector2Int)playerCell;

        selectedBlock = cropManager.GetBlockAt(playerGridLocation);
    }

    /// <summary>
    /// Called via Animation Event when a tool animation completes.
    /// </summary>
    public void OnToolUseFinished()
    {
        isUsingTool = false;
        playerController.CanMove = true;
    }

    private void HandleHarvestTool()
    {
        if (isUsingTool || selectedBlock == null) return;

        if (!selectedBlock.IsFullyGrown()) return;

        isUsingTool = true;
        playerController.CanMove = false;

        animator.SetTrigger("UseHarvest");

        ItemData harvestedItem = selectedBlock.HarvestPlants();

        if (harvestedItem != null && inventoryManager != null)
        {
            inventoryManager.AddItem(harvestedItem, 1);
        }

        cropManager.RemoveFromPlantedCrops(selectedBlock);
    }

    private void HandleHoeTool()
    {
        if (isUsingTool || selectedBlock == null) return;

        if (selectedBlock.IsTilled) return;

        isUsingTool = true;
        playerController.CanMove = false;

        animator.SetTrigger("UseHoe");

        selectedBlock.TillSoil();
    }

    private void HandleWaterTool()
    {
        if (isUsingTool || selectedBlock == null) return;

        if (!selectedBlock.IsTilled || selectedBlock.IsWatered) return;

        isUsingTool = true;
        playerController.CanMove = false;

        animator.SetTrigger("UseWater");

        selectedBlock.WaterSoil();
    }

    private void HandlePlantTool()
    {
        if (isUsingTool || selectedBlock == null) return;

        SeedPacket seed = ToolBarController.ActiveSeedPacket;

        if (seed == null) return;

        if (selectedBlock.CanPlant())
        {
            isUsingTool = true;
            playerController.CanMove = false;

            animator.SetTrigger("UsePlant");

            selectedBlock.PlantSeed(seed);
            cropManager.AddToPlantedCrops(selectedBlock);
        }
    }
}