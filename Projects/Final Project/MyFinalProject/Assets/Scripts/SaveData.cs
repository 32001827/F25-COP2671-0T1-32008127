using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Serializable class for storing game state (Time, Inventory, Crops).
/// </summary>
[System.Serializable]
public class SaveData
{
    public int Day;
    public int Hour;
    public int Minute;

    [System.Serializable]
    public struct InventorySlotData
    {
        public string ItemName;
        public int Quantity;
    }
    public List<InventorySlotData> Inventory = new List<InventorySlotData>();

    [System.Serializable]
    public struct CropData
    {
        public Vector2Int Location;
        public string SeedName;
        public int GrowthStage;
        public float GrowthTimer;
        public bool IsWatered;
        public bool IsTilled;
        public float TilledTimer;
    }
    public List<CropData> Crops = new List<CropData>();
}