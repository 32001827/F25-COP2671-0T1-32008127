using UnityEngine;
using System;

public class DayNightEvents : MonoBehaviour
{
    [Header("Event Times (0-23)")]
    [SerializeField]
    private int sunriseHour = 6;
    [SerializeField]
    private int sunsetHour = 18;

    /// <summary>
    /// Fires when the game time reaches the sunriseHour.
    /// </summary>
    public static event Action OnSunrise;

    /// <summary>
    /// Fires when the game time reaches the sunsetHour.
    /// </summary>
    public static event Action OnSunset;

    private void OnEnable()
    {
        TimeManager.OnGameHourPassed += HandleGameHourPassed;
    }

    private void OnDisable()
    {
        TimeManager.OnGameHourPassed -= HandleGameHourPassed;
    }

    /// <summary>
    /// Listens to the TimeManager and fires sunrise/sunset events at the correct hour.
    /// </summary>
    /// <param name="hour">The current game hour.</param>
    private void HandleGameHourPassed(int hour)
    {
        if (hour == sunriseHour)
        {
            OnSunrise?.Invoke();
        }
        else if (hour == sunsetHour)
        {
            OnSunset?.Invoke();
        }
    }
}