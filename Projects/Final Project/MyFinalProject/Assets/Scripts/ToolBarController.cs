using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.Tilemaps;
using UnityEngine;

public class ToolBarController : MonoBehaviour
{
    public static SeedPacket activeSeedPacket { get; private set; }

    [Header("Seed Selection")]
    [Tooltip("Add all your seed packets here")]
    public List<SeedPacket> allSeedPackets;

    [Tooltip("Toolbar dropdown from UI")]
    public TMP_Dropdown seedDropdown;

    public static event Action OnHoeToolUsed;
    public static event Action OnWaterToolUsed;
    public static event Action OnPlantToolUsed;
    public static event Action OnHarvestToolUsed;

    private void Start()
    {
        PopulateSeedDropdown();
    }

    void PopulateSeedDropdown()
    {
        if (seedDropdown == null || allSeedPackets == null) return;

        List<string> seedNames = allSeedPackets.Select(seed => seed.cropName).ToList();

        seedDropdown.ClearOptions();
        seedDropdown.AddOptions(seedNames);

        seedDropdown.onValueChanged.AddListener(OnSeedSelectionChanged);

        OnSeedSelectionChanged(0);
    }

    public void OnSeedSelectionChanged(int index)
    {
        activeSeedPacket = allSeedPackets[index];
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
