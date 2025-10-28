using UnityEngine;
using UnityEngine.UI;

public class HungerBar : MonoBehaviour
{
    [Header("References")]
    public LogicManagerScript logicManager;   // Connect your LogicManager
    public Slider hungerSlider;               // UI Hunger Bar
    public Slider healthSlider;               // UI Health Bar

    [Header("Hunger Settings")]
    public float maxHunger = 100f;
    
    // ⭐ RATE CHANGED: Was 1f, now 0.5f to make it 50% slower
    public float hungerDepletionRate = 0.5f;    // New hunger depletion rate multiplier (1 is default)

    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float hoursToDepleteHealth = 2f;   // How many in-game hours until health = 0 when starving

    private float currentHunger;
    private float currentHealth;
    private bool isStarving = false;
    private float starvationStartTime = -1f;

    void Start()
    {
        if (logicManager == null)
            logicManager = FindObjectOfType<LogicManagerScript>();

        currentHunger = maxHunger;
        currentHealth = maxHealth;

        hungerSlider.maxValue = maxHunger;
        hungerSlider.value = maxHunger;

        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;
    }

    void Update()
    {
        UpdateHunger();

        if (isStarving)
            TakingHealth();
    }

    private void UpdateHunger()
    {
        if (logicManager == null) return;

        // Apply hunger depletion based on time passed, using hungerDepletionRate
        currentHunger -= hungerDepletionRate * Time.deltaTime;

        // Ensure hunger doesn't go below zero
        currentHunger = Mathf.Clamp(currentHunger, 0f, maxHunger);

        hungerSlider.value = currentHunger;

        // When hunger reaches zero, start health drain
        if (currentHunger <= 0 && !isStarving)
        {
            isStarving = true;
            starvationStartTime = GetTotalGameSeconds();
        }
    }

    private void TakingHealth()
    {
        if (logicManager == null) return;

        float fullDaySeconds = logicManager.minutesInADay * 60f;
        float secondsPerHour = fullDaySeconds / 24f;  // In-game seconds per hour
        float elapsedStarvation = GetTotalGameSeconds() - starvationStartTime;
        float totalDepleteTime = hoursToDepleteHealth * secondsPerHour;
        float healthFraction = Mathf.Clamp01(1f - (elapsedStarvation / totalDepleteTime));

        currentHealth = maxHealth * healthFraction;
        healthSlider.value = currentHealth;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            healthSlider.value = 0;
            Debug.Log("Player has died from starvation!");
        }
    }

    private float GetTotalGameSeconds()
    {
        return (logicManager.daysElapsed * logicManager.minutesInADay * 60f) + logicManager.currentTime;
    }

    /// <summary>
    /// Call this from other scripts to eat food.
    /// </summary>
    /// <param name="hungerAmount">Amount of hunger to restore.</param>
    /// <param name="healthAmount">Amount of health to restore (can be negative).</param>
    public void EatFood(float hungerAmount, float healthAmount)
    {
        // Restore Hunger
        currentHunger += hungerAmount;
        currentHunger = Mathf.Clamp(currentHunger, 0f, maxHunger);
        hungerSlider.value = currentHunger;

        // Restore/Damage Health
        currentHealth += healthAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        healthSlider.value = currentHealth;

        // If we ate, we are no longer starving
        if (currentHunger > 0)
        {
            isStarving = false;
        }

        Debug.Log($"Ate food. Hunger: {currentHunger}, Health: {currentHealth}");
    }
}