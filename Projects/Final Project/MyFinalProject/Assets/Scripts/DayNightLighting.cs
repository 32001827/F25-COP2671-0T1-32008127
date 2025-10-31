using UnityEngine;
using UnityEngine.Rendering.Universal;

// Idiot proof! Make sure we got a light
[RequireComponent(typeof(Light2D))]
public class DayNightLighting : MonoBehaviour
{
    [Header("Lighting Settings")]
    [SerializeField]
    private AnimationCurve intensityCurve;

    [SerializeField]
    private Gradient colorGradient;

    private Light2D globalLight;

    private void Awake()
    {
        globalLight = GetComponent<Light2D>();
    }

    // Subscribe to event
    private void OnEnable()
    {
        TimeManager.OnGameHourPassed += UpdateLighting;
    }

    // Unsubscribe from event
    private void OnDisable()
    {
        TimeManager.OnGameHourPassed -= UpdateLighting;
    }

    /// <summary>
    /// Updates the lighting conditions based on the specified hour of the day.
    /// </summary>
    /// <remarks>This method adjusts the intensity and color of the global light to simulate natural lighting
    /// changes throughout the day.</remarks>
    /// <param name="hour">The current hour of the day, ranging from 0 to 23.</param>
    private void UpdateLighting(int hour)
    {
        // Converts 0-23 hour format to 0.0-1.0 float format
        float normalizedTime = (float)hour / 24f;

        // Evaluate curve/gradient at given time
        float newIntensity = intensityCurve.Evaluate(normalizedTime);
        Color newColor = colorGradient.Evaluate(normalizedTime);

        // Update
        globalLight.intensity = newIntensity;
        globalLight.color = newColor;
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
