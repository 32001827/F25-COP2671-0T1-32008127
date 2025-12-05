using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Updates the UI inventory panel. 
/// Handles static slots that show 0 when empty.
/// </summary>
public class InventoryUI : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The player's inventory manager to read from.")]
    [SerializeField] private InventoryManager inventoryManager;

    [Tooltip("The parent object where slots will be spawned.")]
    [SerializeField] private Transform slotContainer;

    [Tooltip("The prefab for a single inventory slot.")]
    [SerializeField] private GameObject slotPrefab;

    private void Start()
    {
        if (inventoryManager == null)
        {
            inventoryManager = FindFirstObjectByType<InventoryManager>();
        }
        RefreshUI();
    }

    private void OnEnable()
    {
        InventoryManager.OnInventoryChanged += RefreshUI;
    }

    private void OnDisable()
    {
        InventoryManager.OnInventoryChanged -= RefreshUI;
    }

    private void RefreshUI()
    {
        foreach (Transform child in slotContainer)
        {
            Destroy(child.gameObject);
        }

        if (inventoryManager == null) return;

        List<InventoryManager.InventorySlot> inventory = inventoryManager.GetInventory();

        foreach (InventoryManager.InventorySlot slot in inventory)
        {
            GameObject newSlot = Instantiate(slotPrefab, slotContainer);

            Image iconImage = newSlot.transform.Find("Icon")?.GetComponent<Image>();
            if (iconImage != null)
            {
                iconImage.sprite = slot.ItemData.Icon;

                if (slot.Quantity == 0)
                {
                    iconImage.color = new Color(1, 1, 1, 0.5f);
                }
                else
                {
                    iconImage.color = Color.white;
                }
            }

            TextMeshProUGUI qtyText = newSlot.transform.Find("QuantityText")?.GetComponent<TextMeshProUGUI>();
            if (qtyText != null)
            {
                qtyText.text = slot.Quantity.ToString();
            }
        }
    }
}