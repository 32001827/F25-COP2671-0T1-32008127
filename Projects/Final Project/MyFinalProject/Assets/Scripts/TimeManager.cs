using UnityEngine;
using System;

public class TimeManager : MonoBehaviour
{
    // how many real-world seconds equate to a game minute
    [SerializeField]
    private float secondsPerGameMinute = 0.5f;

    // current time declaration
    private float currentGameTimeInMinutes = 0f;

    // current time properties
    public int CurrentDay {  get; private set; }
    public int CurrentHour { get; private set; }
    public int CurrentMinute { get; private set; }

    // Public events
    public static event Action<int> OnGameHourPassed;
    public static event Action<int> OnGameDayPassed;

    // Time tracking vars
    private int lastHour = -1;
    private int lastDay = -1;

    // 1440 minutes in a 24 hour day
    private const float MINUTES_IN_DAY = 1440f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // advance the time
        float minutesToAdd = (Time.deltaTime / secondsPerGameMinute) * 1f;
        currentGameTimeInMinutes += minutesToAdd;

        // check for rollover
        if (currentGameTimeInMinutes >= MINUTES_IN_DAY)
        {
            // reset clock, advance day
            currentGameTimeInMinutes -= MINUTES_IN_DAY;
            CurrentDay++;
        }

        // calculate current hour/minute
        CurrentHour = Mathf.FloorToInt(currentGameTimeInMinutes / 60f);
        CurrentMinute = Mathf.FloorToInt(currentGameTimeInMinutes % 60);

        // check for new hour
        if (lastHour != CurrentHour)
        {
            lastHour = CurrentHour;

            // fire event
            OnGameHourPassed?.Invoke(CurrentHour);
        }

        // check for new day
        if (lastDay != CurrentDay)
        {
            lastDay = CurrentDay;

            // fire event
            OnGameDayPassed?.Invoke(lastDay);
        }
    }
}
