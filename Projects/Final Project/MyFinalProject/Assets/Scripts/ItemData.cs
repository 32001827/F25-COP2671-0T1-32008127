using UnityEngine;

/// <summary>
/// ScriptableObject definition for an inventory item.
/// </summary>
[CreateAssetMenu(fileName = "New Item Data", menuName = "Farm/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("Item Details")]
    [Tooltip("The name of the item as it appears in the UI.")]
    [SerializeField] private string itemName;

    [Tooltip("The icon to display in the inventory UI.")]
    [SerializeField] private Sprite icon;

    [Tooltip("Can this item stack in the inventory?")]
    [SerializeField] private bool isStackable = true;

    public string ItemName => itemName;
    public Sprite Icon => icon;
    public bool IsStackable => isStackable;
}