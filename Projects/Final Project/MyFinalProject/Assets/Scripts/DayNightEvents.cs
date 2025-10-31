using UnityEngine;
using System;
using System.Runtime.CompilerServices;

public class DayNightEvents : MonoBehaviour
{

    [Header("Event Times (0-23)")]
    [SerializeField]
    private int sunriseHour = 6;
    [SerializeField]
    private int sunsetHour = 18;

    public static event Action OnSunrise;
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
