using UnityEngine;
using TMPro;

/// <summary>
/// Updates the UI text elements to display the current day and time.
/// </summary>
public class TimeDisplayUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private TextMeshProUGUI hourText;
    [SerializeField] private TimeManager timeManager;

    private void Start()
    {
        if (timeManager == null)
        {
            timeManager = FindFirstObjectByType<TimeManager>();
        }

        UpdateVisuals();
    }

    private void OnEnable()
    {
        TimeManager.OnGameMinutePassed += HandleGameMinutePassed;
        TimeManager.OnGameDayPassed += HandleGameDayPassed;
    }

    private void OnDisable()
    {
        TimeManager.OnGameMinutePassed -= HandleGameMinutePassed;
        TimeManager.OnGameDayPassed -= HandleGameDayPassed;
    }

    private void HandleGameMinutePassed(int minute)
    {
        UpdateVisuals();
    }

    private void HandleGameDayPassed(int day)
    {
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (timeManager == null) return;

        if (dayText != null)
        {
            dayText.text = $"Day: {timeManager.CurrentDay}";
        }

        if (hourText != null)
        {
            hourText.text = $"{timeManager.CurrentHour:00}:{timeManager.CurrentMinute:00}";
        }
    }
}