using UnityEngine;

[CreateAssetMenu(fileName = "New Seed Packet", menuName = "Farm/Seed Packet")]
public class SeedPacket : ScriptableObject
{
    [Header("Seed Packet Info")]
    [SerializeField] private string cropName;
    [SerializeField] private Sprite icon;

    [Header("Crop Growth")]
    [SerializeField] private Sprite[] growthSprites;
    [SerializeField]
    [Tooltip("Total time (in game minutes) for the crop to fully grow.")]
    private int growthTimeInMinutes;

    [Header("Harvest")]
    [SerializeField] private ItemData itemYield;

    public string CropName => cropName;
    public Sprite Icon => icon;
    public Sprite[] GrowthSprites => growthSprites;
    public int GrowthTimeInMinutes => growthTimeInMinutes;
    public ItemData ItemYield => itemYield;
}