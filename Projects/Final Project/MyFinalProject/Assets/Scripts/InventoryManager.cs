using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the player's inventory using a static list of all possible items.
/// </summary>
public class InventoryManager : MonoBehaviour
{
    /// <summary>
    /// Represents a single slot in the inventory.
    /// </summary>
    [System.Serializable]
    public class InventorySlot
    {
        [SerializeField] private ItemData itemData;
        [SerializeField] private int quantity;

        public ItemData ItemData => itemData;
        public int Quantity => quantity;

        public InventorySlot(ItemData item, int qty)
        {
            itemData = item;
            quantity = qty;
        }

        public void AddQuantity(int amount)
        {
            quantity += amount;
        }

        public void SetQuantity(int amount)
        {
            quantity = amount;
        }
    }

    [Header("Configuration")]
    [Tooltip("Drag ALL your ItemData assets here. These will form the static inventory bar.")]
    [SerializeField] private List<ItemData> allGameItems;

    [SerializeField] private List<InventorySlot> inventory = new List<InventorySlot>();

    /// <summary>
    /// Fires whenever the inventory content changes.
    /// </summary>
    public static event Action OnInventoryChanged;

    private void Start()
    {
        if (inventory.Count == 0)
        {
            InitializeInventory();
        }
    }

    private void InitializeInventory()
    {
        inventory.Clear();

        if (allGameItems == null) return;

        foreach (ItemData item in allGameItems)
        {
            inventory.Add(new InventorySlot(item, 0));
        }

        OnInventoryChanged?.Invoke();
    }

    /// <summary>
    /// Adds an item to the existing static slot.
    /// </summary>
    /// <param name="item">The item data to add.</param>
    /// <param name="amount">The quantity to add.</param>
    public void AddItem(ItemData item, int amount = 1)
    {
        InventorySlot existingSlot = inventory.Find(slot => slot.ItemData == item);

        if (existingSlot != null)
        {
            existingSlot.AddQuantity(amount);
            OnInventoryChanged?.Invoke();
        }
    }

    /// <summary>
    /// Retrieves the current inventory list.
    /// </summary>
    /// <returns>A list of inventory slots.</returns>
    public List<InventorySlot> GetInventory()
    {
        return inventory;
    }

    /// <summary>
    /// Loads inventory state from save data.
    /// </summary>
    /// <param name="savedSlots">The list of saved slot data.</param>
    public void LoadInventory(List<SaveData.InventorySlotData> savedSlots)
    {
        InitializeInventory();

        foreach (var savedSlot in savedSlots)
        {
            InventorySlot realSlot = inventory.Find(s => s.ItemData.ItemName == savedSlot.ItemName);

            if (realSlot != null)
            {
                realSlot.SetQuantity(savedSlot.Quantity);
            }
        }
        OnInventoryChanged?.Invoke();
    }
}