using UnityEngine;

/// <summary>
/// Represents an object in the world that can be picked up and added to inventory.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class Harvestable : MonoBehaviour
{
    [Header("Item Configuration")]
    [Tooltip("The ItemData this object represents.")]
    [SerializeField] private ItemData itemData;

    [Tooltip("How many of the item to give.")]
    [SerializeField] private int quantity = 1;

    [Tooltip("If true, the object destroys itself on pickup.")]
    [SerializeField] private bool destroyOnPickup = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            InventoryManager inventory = other.GetComponent<InventoryManager>();

            if (inventory != null)
            {
                inventory.AddItem(itemData, quantity);

                if (destroyOnPickup)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}