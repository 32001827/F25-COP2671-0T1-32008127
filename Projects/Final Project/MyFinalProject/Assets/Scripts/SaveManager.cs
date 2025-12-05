using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles saving and loading the game state to a local file.
/// </summary>
public class SaveManager : SingletonMonoBehaviour<SaveManager>
{
    [Header("References")]
    [SerializeField] private TimeManager timeManager;
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private CropManager cropManager;

    private string SavePath => Application.persistentDataPath + "/savegame.json";

    private void Start()
    {
        if (File.Exists(SavePath))
        {
            LoadGame();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            SaveGame();
        }

        if (Input.GetKeyDown(KeyCode.F6))
        {
            LoadGame();
        }

        if (Input.GetKeyDown(KeyCode.F7))
        {
            NewGame();
        }
    }

    /// <summary>
    /// Deletes the save file and restarts the scene.
    /// </summary>
    public void NewGame()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Serializes current game state and writes to file.
    /// </summary>
    public void SaveGame()
    {
        SaveData data = new SaveData();

        data.Day = timeManager.CurrentDay;
        data.Hour = timeManager.CurrentHour;
        data.Minute = timeManager.CurrentMinute;

        var slots = inventoryManager.GetInventory();
        foreach (var slot in slots)
        {
            if (slot.ItemData != null)
            {
                data.Inventory.Add(new SaveData.InventorySlotData
                {
                    ItemName = slot.ItemData.ItemName,
                    Quantity = slot.Quantity
                });
            }
        }

        var cropBlocks = cropManager.GetAllCrops();
        foreach (var block in cropBlocks)
        {
            data.Crops.Add(new SaveData.CropData
            {
                Location = block.Location,
                SeedName = block.SeedPacket != null ? block.SeedPacket.CropName : "",
                GrowthStage = block.CurrentGrowthStage,
                GrowthTimer = block.GrowthTimer,
                IsWatered = block.IsWatered,
                IsTilled = block.IsTilled,
                TilledTimer = block.TilledTimer
            });
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
    }

    /// <summary>
    /// Reads from file and restores game state.
    /// </summary>
    public void LoadGame()
    {
        if (!File.Exists(SavePath))
        {
            return;
        }

        string json = File.ReadAllText(SavePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        timeManager.SetTime(data.Day, data.Hour, data.Minute);
        inventoryManager.LoadInventory(data.Inventory);
        cropManager.LoadCrops(data.Crops);
    }
}