using UnityEngine;
using UnityEngine.Tilemaps;

public class FarmingController : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private CropManager cropManager;

    [SerializeField]
    private Tilemap farmingTilemap;

    [Header("Testing")]
    [Tooltip("A seed packet to test planting")]
    public SeedPacket testSeedPacket;

    private CropBlock selectedBlock;
    private Vector2Int playerGridLocation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3Int playerCell = farmingTilemap.WorldToCell(transform.position);
        playerGridLocation = (Vector2Int)playerCell;

        selectedBlock = cropManager.GetBlockAt(playerGridLocation);

        HandleFarmingInput();
    }

    private void HandleFarmingInput()
    {

        if (selectedBlock == null)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            selectedBlock.TillSoil();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            selectedBlock.WaterSoil();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            selectedBlock.PlantSeed(testSeedPacket);
            cropManager.AddToPlantedCrops(selectedBlock);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            selectedBlock.HarvestPlants();
            cropManager.RemoveFromPlantedCrops(selectedBlock);
        }
    }
}
