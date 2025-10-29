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
    public float fullnessHoursPerMeal = 2f;   // How many in-game hours fullness lasts

    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float hoursToDepleteHealth = 2f;   // How many in-game hours until health = 0 when starving

    private float currentHunger;
    private float currentHealth;
    private bool isStarving = false;
    private float starvationStartTime = -1f;
    private float fullnessTimer = 0f;         // Countdown timer for fullness (in real-world seconds)

    void Start()
    {
        // Safety check for LogicManager
        if (logicManager == null)
        {
            logicManager = FindAnyObjectByType<LogicManagerScript>();
            if (logicManager == null)
            {
                Debug.LogError("LogicManager is not assigned or found!", this);
                enabled = false; // Disable script if LogicManager is missing
                return;
            }
        }
        // Safety checks for Sliders
        if (hungerSlider == null) Debug.LogError("Hunger Slider is not assigned!", this);
        if (healthSlider == null) Debug.LogError("Health Slider is not assigned!", this);


        currentHunger = maxHunger;
        currentHealth = maxHealth;

        if (hungerSlider != null)
        {
            hungerSlider.maxValue = maxHunger;
            hungerSlider.value = maxHunger;
        }
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = maxHealth;
        }
    }

    void Update()
    {
        UpdateHunger();

        if (isStarving)
            TakingHealth(); // Deplete health when starving
    }

    private void UpdateHunger()
    {
        if (logicManager == null || hungerSlider == null) return; // Need logicManager for time

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

        // Ensure hunger doesn't go below zero
        currentHunger = Mathf.Clamp(currentHunger, 0f, maxHunger);

        hungerSlider.value = currentHunger;

        // When hunger reaches zero, start health drain
        if (currentHunger <= 0 && !isStarving)
        {
            isStarving = true;
            starvationStartTime = GetTotalGameSeconds();
            Debug.Log("Player is now starving!");
        }
    }

    // This function depletes health OVER TIME when starving
    private void TakingHealth()
    {
        if (logicManager == null || healthSlider == null) return;

        // Only run if actually starving
        if (!isStarving || currentHunger > 0)
        {
            isStarving = false; // Stop starving if hunger is restored
            return;
        }

        float fullDaySeconds = logicManager.minutesInADay * 60f;
        float secondsPerHour = fullDaySeconds / 24f;  // In-game seconds per hour

        // Ensure valid calculation if hoursToDepleteHealth is zero or negative
        if (hoursToDepleteHealth <= 0) hoursToDepleteHealth = 0.1f;
        float totalDepleteTimeInGameSeconds = hoursToDepleteHealth * secondsPerHour;

        // Ensure we don't divide by zero if secondsPerHour is somehow zero
        if (totalDepleteTimeInGameSeconds <= 0) return;

        float elapsedStarvation = GetTotalGameSeconds() - starvationStartTime;

        // Calculate health lost per real-world second
        float healthLossPerSecond = maxHealth / totalDepleteTimeInGameSeconds;

        // Apply damage based on real time passed
        ModifyHealth(-healthLossPerSecond * Time.deltaTime); // Use ModifyHealth to handle clamping and UI
    }

    private float GetTotalGameSeconds()
    {
        // Check if logicManager is available
        if (logicManager == null) return Time.time; // Fallback to real time if logic manager missing

        // Calculate total game time in seconds
        return (logicManager.daysElapsed * logicManager.minutesInADay * 60f) + logicManager.currentTime;
    }

    /// <summary>
    /// Call this from other scripts to eat food.
    /// </summary>
    public void EatFood(float hungerAmount, float healthAmount)
    {
        // Restore Hunger
        currentHunger += hungerAmount;
        currentHunger = Mathf.Clamp(currentHunger, 0f, maxHunger);
        if (hungerSlider != null) hungerSlider.value = currentHunger;

        // Restore/Damage Health using the ModifyHealth function
        ModifyHealth(healthAmount);

        // --- Fullness Logic ---
        if (logicManager != null && fullnessHoursPerMeal > 0 && hungerAmount > 0) // Only add fullness if restoring hunger
        {
            float fullDaySeconds = logicManager.minutesInADay * 60f;
            float secondsPerHour = fullDaySeconds / 24f;
            if (secondsPerHour > 0) // Avoid division by zero
            {
                float fullnessInSeconds = fullnessHoursPerMeal * secondsPerHour;
                fullnessTimer += fullnessInSeconds; // Add duration
            }
        }

        // If we ate enough, we are no longer starving
        if (currentHunger > 0)
        {
            isStarving = false;
        }

        Debug.Log($"Ate food. Hunger: {currentHunger:F1}, Health: {currentHealth:F1}");
    }

    // --- ⭐ NEW FUNCTION TO MODIFY HEALTH ---
    /// <summary>
    /// Modifies the player's current health by the given amount.
    /// Use negative values to deal damage. Handles UI update and death check.
    /// </summary>
    public void ModifyHealth(float amount)
    {
        // Apply change
        currentHealth += amount;

        // Clamp health between 0 and maxHealth
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        // Update the UI slider
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }
        else
        {
            Debug.LogWarning("Health Slider is missing - cannot update UI.", this);
        }

        // Check for death
        if (currentHealth <= 0)
        {
            // Prevent spamming death message
            if (healthSlider == null || healthSlider.value > 0) // Crude check if already dead
            {
                 Debug.Log("Player has died!");
                 // Add game over logic here
                 // Example: Time.timeScale = 0; // Pause game
                 // Example: FindObjectOfType<GameManager>().LoadGameOverScreen();
            }
        }
    }
    // --- ⭐ END NEW FUNCTION ---
}