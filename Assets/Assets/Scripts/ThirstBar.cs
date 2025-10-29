using UnityEngine;
using UnityEngine.UI;

public class ThirstBar : MonoBehaviour
{
    [Header("References")]
    public LogicManagerScript logicManager;   // Connect your LogicManager
    public Slider thirstSlider;               // UI Thirst Bar
    public Slider healthSlider;               // UI Health Bar

    [Header("Thirst Settings")]
    public float maxThirst = 100f;
    public float daysToDeplete = 0.1f;          // Time for thirst to hit zero (in game days)

    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float hoursToDepleteHealth = 2f;   // How many in-game hours until health = 0 when dehydrated

    private float currentThirst;
    private float currentHealth;
    private bool isDehydrated = false;
    private float dehydrationStartTime = -1f;

    void Start()
    {
        if (logicManager == null)
            logicManager = FindAnyObjectByType<LogicManagerScript>();

        currentThirst = maxThirst;
        currentHealth = maxHealth;

        thirstSlider.maxValue = maxThirst;
        thirstSlider.value = maxThirst;

        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;
    }

    void Update()
    {
        UpdateThirst();

        if (isDehydrated)
            TakingHealth();
    }

    private void UpdateThirst()
    {
        if (logicManager == null) return;

        // Calculate total in-game days passed
        float totalDaysPassed = logicManager.daysElapsed 
                              + (logicManager.currentTime / (logicManager.minutesInADay * 60f));

        // Thirst drains linearly over chosen number of in-game days
        float remainingFraction = Mathf.Clamp01(1f - (totalDaysPassed / daysToDeplete));

        currentThirst = maxThirst * remainingFraction;
        thirstSlider.value = currentThirst;

        // When thirst reaches zero, start health drain
        if (currentThirst <= 0 && !isDehydrated)
        {
            isDehydrated = true;
            dehydrationStartTime = GetTotalGameSeconds();
        }
    }

    private void TakingHealth()
    {
        if (logicManager == null) return;

        // 1 in-game day = minutesInADay * 60 seconds
        float fullDaySeconds = logicManager.minutesInADay * 60f;
        float secondsPerHour = fullDaySeconds / 24f;  // In-game seconds per hour

        // How long has the player been dehydrated (in in-game seconds)
        float elapsedDehydration = GetTotalGameSeconds() - dehydrationStartTime;

        // Drain health linearly over hoursToDepleteHealth hours
        float totalDepleteTime = hoursToDepleteHealth * secondsPerHour;
        float healthFraction = Mathf.Clamp01(1f - (elapsedDehydration / totalDepleteTime));

        currentHealth = maxHealth * healthFraction;
        healthSlider.value = currentHealth;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            healthSlider.value = 0;
            // Optionally trigger player death or game over here
            Debug.Log("Player has died from dehydration!");
        }
    }

    private float GetTotalGameSeconds()
    {
        // Converts days + time into total in-game seconds
        return (logicManager.daysElapsed * logicManager.minutesInADay * 60f) + logicManager.currentTime;
    }
}
