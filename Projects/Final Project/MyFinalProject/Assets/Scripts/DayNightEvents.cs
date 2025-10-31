using UnityEngine;
using System;
using System.Runtime.CompilerServices;

public class DayNightEvents : MonoBehaviour
{
    // Configuration
    [Header("Event Times (0-23)")]
    [SerializeField]
    private int sunriseHour = 6;
    [SerializeField]
    private int sunsetHour = 18;

    // Public events
    public static event Action OnSunrise;
    public static event Action OnSunset;

    // Subscribe to event
    private void OnEnable()
    {
        TimeManager.OnGameHourPassed += HandleGameHourPassed;
    }

    // Unsubscribe to event
    private void OnDisable()
    {
        TimeManager.OnGameHourPassed -= HandleGameHourPassed;
    }

    /// <summary>
    /// Handles the event when a game hour has passed, triggering sunrise or sunset events.
    /// </summary>
    /// <param name="hour">The current hour in the game, used to determine if it is sunrise or sunset.</param>
    private void HandleGameHourPassed(int hour)
    {
        // check and fire events
        if (hour == sunriseHour)
        {
            OnSunrise?.Invoke();
        }
        else if (hour == sunsetHour)
        {
            OnSunset?.Invoke();
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
