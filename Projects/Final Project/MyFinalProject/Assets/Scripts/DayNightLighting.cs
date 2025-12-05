using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Controls a global 2D light to simulate day/night cycles using colors and intensity.
/// </summary>
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

    private void OnEnable()
    {
        TimeManager.OnGameHourPassed += UpdateLighting;
    }

    private void OnDisable()
    {
        TimeManager.OnGameHourPassed -= UpdateLighting;
    }

    /// <summary>
    /// Updates the global 2D light's intensity and color based on the game hour.
    /// </summary>
    /// <param name="hour">The current game hour.</param>
    private void UpdateLighting(int hour)
    {
        float normalizedTime = (float)hour / 24f;

        float newIntensity = intensityCurve.Evaluate(normalizedTime);
        Color newColor = colorGradient.Evaluate(normalizedTime);

        globalLight.intensity = newIntensity;
        globalLight.color = newColor;
    }
}