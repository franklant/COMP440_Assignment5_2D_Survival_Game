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
    public float hungerDepletionRate = 0.5f;
    public float fullnessHoursPerMeal = 2f;   // ⭐ How many in-game hours fullness lasts

    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float hoursToDepleteHealth = 2f;   // How many in-game hours until health = 0 when starving

    private float currentHunger;
    private float currentHealth;
    private bool isStarving = false;
    private float starvationStartTime = -1f;
    private float fullnessTimer = 0f;         // ⭐ Countdown timer for fullness (in real-world seconds)

    void Start()
    {
        if (logicManager == null)
            logicManager = FindAnyObjectByType<LogicManagerScript>();

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

        // ⭐ --- MODIFIED HUNGER LOGIC --- ⭐
        // Check if fullness timer is active
        if (fullnessTimer > 0)
        {
            // If full, just count down the timer and do NOT deplete hunger
            fullnessTimer -= Time.deltaTime;
        }
        else
        {
            // If not full, deplete hunger as normal
            currentHunger -= hungerDepletionRate * Time.deltaTime;
        }
        // ⭐ --- END OF MODIFIED LOGIC --- ⭐


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
        
        // ⭐ --- NEW "FULLNESS" LOGIC --- ⭐
        // We need the logicManager to do the time conversion
        if (logicManager != null)
        {
            // Calculate how many REAL seconds our in-game hours is
            float fullDaySeconds = logicManager.minutesInADay * 60f;
            float secondsPerHour = fullDaySeconds / 24f;
            float fullnessInSeconds = fullnessHoursPerMeal * secondsPerHour;

            // Add this duration to the timer (+= makes it stack if you eat twice)
            fullnessTimer += fullnessInSeconds;
        }
        // ⭐ --- END OF NEW LOGIC --- ⭐


        // If we ate, we are no longer starving
        if (currentHunger > 0)
        {
            isStarving = false;
        }

        Debug.Log($"Ate food. Hunger: {currentHunger}, Health: {currentHealth}");
    }
}