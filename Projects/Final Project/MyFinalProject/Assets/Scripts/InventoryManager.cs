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
    }

    [Header("Configuration")]
    [Tooltip("Drag ALL your ItemData assets here. These will form the static inventory bar.")]
    [SerializeField] private List<ItemData> allGameItems;

    [SerializeField] private List<InventorySlot> inventory = new List<InventorySlot>();

    public static event Action OnInventoryChanged;

    private void Start()
    {
        InitializeInventory();
    }

    /// <summary>
    /// Pre-fills the inventory with every item in the allGameItems list, set to 0 quantity.
    /// </summary>
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
    /// <param name="amount">The amount to add.</param>
    public void AddItem(ItemData item, int amount = 1)
    {
        InventorySlot existingSlot = inventory.Find(slot => slot.ItemData == item);

        if (existingSlot != null)
        {
            existingSlot.AddQuantity(amount);
            OnInventoryChanged?.Invoke();
        }
        else
        {
            Debug.LogWarning($"Item '{item.ItemName}' was collected but is not in the 'All Game Items' list on InventoryManager!");
        }
    }

    /// <summary>
    /// Returns the current list of inventory slots.
    /// </summary>
    public List<InventorySlot> GetInventory()
    {
        return inventory;
    }
}