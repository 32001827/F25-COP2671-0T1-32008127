using UnityEngine;

[CreateAssetMenu(fileName = "New Seed Packet", menuName = "Farm/Seed Packet")]
public class SeedPacket : ScriptableObject
{
    [Header("Seed Packet Info")]
    public string cropName;
    public Sprite icon;

    [Header("Crop Growth")]
    public Sprite[] growthSprites;
    [Tooltip("Total time (in game minutes) for the crop to fully grow.")]
    public int growthTimeInMinutes;

    [Header("Harvest")]
    public Harvestable harvestablePrefab;
}
