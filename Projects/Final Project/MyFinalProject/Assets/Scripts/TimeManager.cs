using UnityEngine;
using System;

public class TimeManager : MonoBehaviour
{
    [Tooltip("How many real-world seconds equate to one in-game minute")]
    [SerializeField]
    private float secondsPerGameMinute = 0.5f;

    private float currentGameTimeInMinutes = 0f;

    public int CurrentDay { get; private set; }
    public int CurrentHour { get; private set; }
    public int CurrentMinute { get; private set; }

    /// <summary>
    /// Fires every time a new in-game hour passes.
    /// </summary>
    public static event Action<int> OnGameHourPassed;

    /// <summary>
    /// Fires every time a new in-game day passes.
    /// </summary>
    public static event Action<int> OnGameDayPassed;

    /// <summary>
    /// Fires every time a new in-game minute passes.
    /// </summary>
    public static event Action<int> OnGameMinutePassed;

    private int lastHour = -1;
    private int lastDay = -1;
    private int lastMinute = -1;

    private const float kMinutesInDay = 1440f;

    void Update()
    {
        float minutesToAdd = (Time.deltaTime / secondsPerGameMinute);
        currentGameTimeInMinutes += minutesToAdd;

        CurrentMinute = Mathf.FloorToInt(currentGameTimeInMinutes % 60);

        if (lastMinute != CurrentMinute)
        {
            lastMinute = CurrentMinute;
            OnGameMinutePassed?.Invoke(CurrentMinute);
        }

        if (currentGameTimeInMinutes >= kMinutesInDay)
        {
            currentGameTimeInMinutes -= kMinutesInDay;
            CurrentDay++;

            if (lastDay != CurrentDay)
            {
                lastDay = CurrentDay;
                OnGameDayPassed?.Invoke(CurrentDay);
            }
        }

        CurrentHour = Mathf.FloorToInt(currentGameTimeInMinutes / 60f);
        CurrentMinute = Mathf.FloorToInt(currentGameTimeInMinutes % 60);

        if (lastHour != CurrentHour)
        {
            lastHour = CurrentHour;
            OnGameHourPassed?.Invoke(CurrentHour);
        }
    }
}