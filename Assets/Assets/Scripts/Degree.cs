using UnityEngine;
using UnityEngine.UI;  // Include this to access UI Text

public class TemperatureManagerScript : MonoBehaviour
{
    [Header("References")]
    public LogicManagerScript logicManager;   // Reference to your LogicManagerScript
    public Text temperatureText;              // UI Text to display the temperature

    [Header("Temperature Settings")]
    public float minTemperature = 10f;        // Minimum temperature (e.g., night)
    public float maxTemperature = 30f;        // Maximum temperature (e.g., noon)
    public float transitionTime = 1f;         // Time for temperature to transition (in days)

    private float currentTemperature;

    void Start()
    {
        if (logicManager == null)
            logicManager = FindAnyObjectByType<LogicManagerScript>();

        // Initialize the temperature at the start of the game
        currentTemperature = minTemperature;

        // Ensure the temperature text is updated at the start
        UpdateTemperatureText();
    }

    void Update()
    {
        UpdateTemperature();
    }

    void UpdateTemperature()
    {
        if (logicManager == null) return;

        // Calculate how much time has passed in a day as a fraction
        float totalElapsedTime = logicManager.daysElapsed + (logicManager.currentTime / (logicManager.minutesInADay * 60f));

        // Temperature change based on the time of day
        float temperatureFactor = Mathf.PingPong(totalElapsedTime / transitionTime, 1f);

        // Temperature transitions from min to max between night and noon
        currentTemperature = Mathf.Lerp(minTemperature, maxTemperature, temperatureFactor);

        // Update the UI Text with the current temperature in degrees
        UpdateTemperatureText();
    }

    void UpdateTemperatureText()
    {
        if (temperatureText != null)
        {
            // Update the temperature text on the UI
            temperatureText.text = "Temperature: " + currentTemperature.ToString("F1") + "�C"; // Shows 1 decimal point
        }
    }

    public float GetCurrentTemperature()
    {
        return currentTemperature;
    }
}
