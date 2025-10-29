using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;  // Add this for scene management

public class HealthBar : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;         // Max health value
    public int currentHealth;           // Current health value
    public Slider slider;               // Reference to the health bar slider

    void Start()
    {
        // Set up the initial health values
        SetMaxHealth(maxHealth);
    }

    // Set the maximum health value and initialize slider
    public void SetMaxHealth(int health)
    {
        maxHealth = health;
        slider.maxValue = health;       // Set slider's max value
        slider.value = health;          // Set slider's initial value to max health
        currentHealth = health;         // Initialize current health to max health
    }

    // Update the current health value
    public void SetHealth(int health)
    {
        currentHealth = Mathf.Clamp(health, 0, maxHealth);  // Ensure health is within the valid range
        slider.value = currentHealth;  // Update slider value to match current health

        // If health reaches zero, trigger Game Over
        if (currentHealth == 0)
        {
            TriggerGameOver();  // Call Game Over method when health is zero
        }
    }

    // Optionally, call this method to reduce health over time or in response to damage
    public void TakeDamage(int damage)
    {
        SetHealth(currentHealth - damage);  // Reduce health by damage value
    }

    // Optionally, call this method to heal the player
    public void Heal(int amount)
    {
        SetHealth(currentHealth + amount);  // Increase health by heal amount
    }

    // Method to trigger the Game Over scene
    void TriggerGameOver()
    {
        // Optionally, add a delay to allow for visual effects, then load the Game Over scene
        Debug.Log("Player has died!");

        // Load the Game Over scene (ensure this scene is in your Build Settings)
        SceneManager.LoadScene("GameOver.");  // Replace "GameOverScene" with your actual scene name
    }
}
