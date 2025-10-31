using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class DayNightLighting : MonoBehaviour
{
    [Header("Lighting Settings")]
    [SerializeField]
    private AnimationCurve intensityCurve;

    [SerializeField]
    private Gradient colorGradient;

    [SerializeField]
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

    private void UpdateLighting(int hour)
    {
        float normalizedTime = (float)hour / 24f;

        float newIntensity = intensityCurve.Evaluate(normalizedTime);
        Color newColor = colorGradient.Evaluate(normalizedTime);

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
