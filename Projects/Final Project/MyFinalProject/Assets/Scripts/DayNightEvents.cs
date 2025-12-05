using UnityEngine;
using System;

/// <summary>
/// Triggers events when the game time reaches specific hours (Sunrise/Sunset).
/// </summary>
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