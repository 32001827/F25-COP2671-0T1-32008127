using UnityEngine;
using UnityEngine.Tilemaps;

public class FarmingController : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private CropManager cropManager;

    [SerializeField]
    private Tilemap farmingTilemap;

    [Header("Animation")]
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private PlayerController playerController;

    private CropBlock selectedBlock;
    private Vector2Int playerGridLocation;

    private bool isUsingTool = false;

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
        if (isUsingTool) return;

        Vector3Int playerCell = farmingTilemap.WorldToCell(transform.position);
        playerGridLocation = (Vector2Int)playerCell;

        selectedBlock = cropManager.GetBlockAt(playerGridLocation);
    }

    public void OnToolUseFinished()
    {
        isUsingTool = false;
        playerController.canMove = true;
    }

    private void HandleHoeTool()
    {
        if (isUsingTool || selectedBlock == null) return;

        isUsingTool = true;
        playerController.canMove = false;

        animator.SetTrigger("UseHoe");

        selectedBlock.TillSoil(); 
    }

    private void HandleWaterTool()
    {
        if (isUsingTool || selectedBlock == null) return;

        isUsingTool = true;
        playerController.canMove = false;

        animator.SetTrigger("UseWater");

        selectedBlock.WaterSoil(); 
    }

    private void HandlePlantTool()
    {
        if (isUsingTool || selectedBlock == null) return;

        SeedPacket seed = ToolBarController.activeSeedPacket; 

        if (seed == null) return;

        if (selectedBlock.CanPlant())
        {
            isUsingTool = true;
            playerController.canMove = false;

            animator.SetTrigger("UsePlant");

            selectedBlock.PlantSeed(seed); 
            cropManager.AddToPlantedCrops(selectedBlock); 
        }
    }

    private void HandleHarvestTool()
    {
        if (isUsingTool || selectedBlock == null) return;

        isUsingTool = true;
        playerController.canMove = false;

        animator.SetTrigger("UseHarvest");

        selectedBlock.HarvestPlants(); 
        cropManager.RemoveFromPlantedCrops(selectedBlock); 
    }

}
