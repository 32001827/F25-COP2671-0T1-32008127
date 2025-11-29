using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ToolBarController : MonoBehaviour
{
    /// <summary>
    /// The currently selected SeedPacket from the dropdown.
    /// </summary>
    public static SeedPacket ActiveSeedPacket { get; private set; }

    [Header("Seed Selection")]
    [Tooltip("Add all your seed packets here")]
    [SerializeField] private List<SeedPacket> allSeedPackets;

    [Tooltip("Toolbar dropdown from UI")]
    [SerializeField] private TMP_Dropdown seedDropdown;

    public static event Action OnHoeToolUsed;
    public static event Action OnWaterToolUsed;
    public static event Action OnPlantToolUsed;
    public static event Action OnHarvestToolUsed;

    private void Start()
    {
        PopulateSeedDropdown();
    }

    /// <summary>
    /// Fills the seed dropdown UI with the names of available seeds.
    /// </summary>
    void PopulateSeedDropdown()
    {
        if (seedDropdown == null || allSeedPackets == null) return;

        List<string> seedNames = allSeedPackets.Select(seed => seed.CropName).ToList();

        seedDropdown.ClearOptions();
        seedDropdown.AddOptions(seedNames);

        seedDropdown.onValueChanged.AddListener(OnSeedSelectionChanged);

        OnSeedSelectionChanged(0);
    }

    /// <summary>
    /// Called when the dropdown value changes. Sets the active seed.
    /// </summary>
    /// <param name="index">The new index of the dropdown.</param>
    public void OnSeedSelectionChanged(int index)
    {
        if (index < 0 || index >= allSeedPackets.Count) return;

        ActiveSeedPacket = allSeedPackets[index];
    }

    public void HoeButtonPressed()
    {
        OnHoeToolUsed?.Invoke();
    }

    public void WaterButtonPressed()
    {
        OnWaterToolUsed?.Invoke();
    }

    public void SeedButtonPressed()
    {
        OnPlantToolUsed?.Invoke();
    }

    public void GatherButtonPressed()
    {
        OnHarvestToolUsed?.Invoke();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            HoeButtonPressed();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            WaterButtonPressed();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SeedButtonPressed();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            GatherButtonPressed();
        }
    }
}