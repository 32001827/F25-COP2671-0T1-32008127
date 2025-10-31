using UnityEngine;
using System;

public class TimeManager : MonoBehaviour
{
    [SerializeField]
    private float secondsPerGameMinute = 0.5f;

    private float currentGameTimeInMinutes = 0f;

    public int CurrentDay {  get; private set; }
    public int CurrentHour { get; private set; }
    public int CurrentMinute { get; private set; }

    public static event Action<int> OnGameHourPassed;
    public static event Action<int> OnGameDayPassed;

    private int lastHour = -1;
    private int lastDay = -1;

    private const float MINUTES_IN_DAY = 1440f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float minutesToAdd = (Time.deltaTime / secondsPerGameMinute) * 1f;
        currentGameTimeInMinutes += minutesToAdd;

        if (currentGameTimeInMinutes >= MINUTES_IN_DAY)
        {
            currentGameTimeInMinutes -= MINUTES_IN_DAY;
            CurrentDay++;
        }

        CurrentHour = Mathf.FloorToInt(currentGameTimeInMinutes / 60f);
        CurrentMinute = Mathf.FloorToInt(currentGameTimeInMinutes % 60);

        if (lastHour != CurrentHour)
        {
            lastHour = CurrentHour;

            OnGameHourPassed?.Invoke(CurrentHour);
        }

        if (lastDay != CurrentDay)
        {

            lastDay = CurrentDay;

            OnGameDayPassed?.Invoke(lastDay);
        }
    }
}
