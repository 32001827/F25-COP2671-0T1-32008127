using UnityEngine;

/// <summary>
/// Represents an object in the world that can be picked up and added to inventory.
/// </summary>
[RequireComponent(typeof(Collider2D))] // Ensures it has a collider for triggers
public class Harvestable : MonoBehaviour
{
    [Header("Item Configuration")]
    [Tooltip("The ItemData this object represents (e.g., Corn).")]
    [SerializeField] private ItemData itemData;

    [Tooltip("How many of the item to give.")]
    [SerializeField] private int quantity = 1;

    [Tooltip("If true, the object destroys itself on pickup.")]
    [SerializeField] private bool destroyOnPickup = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object colliding is the Player
        if (other.CompareTag("Player"))
        {
            // Try to get the InventoryManager from the player
            InventoryManager inventory = other.GetComponent<InventoryManager>();

            if (inventory != null)
            {
                // Add the item to the player's inventory
                inventory.AddItem(itemData, quantity);

                if (destroyOnPickup)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}