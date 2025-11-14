using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HealthBar : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;
    public float realHealth; // new variable that mirrors slider value
    public Slider slider;
    public GameObject deathCutscene;
    private string sceneToLoad = "DeathScene";

    void Start()
    {
        SetMaxHealth(maxHealth);
        deathCutscene.SetActive(false);

        
    }

    void Update()
    {
        // Keep realHealth synced with the slider’s current value every frame
        realHealth = slider.value;

    if (realHealth == 0)
        {
            Die(); // this calls once the health is at zero
        }
    }

    public void SetMaxHealth(int health)
    {
        maxHealth = health;
        slider.maxValue = health;
        currentHealth = health;
        slider.value = health;

        realHealth = slider.value; // initialize realHealth to match
    }

    public void SetHealth(int health)
    {
        currentHealth = Mathf.Clamp(health, 0, maxHealth);
        slider.value = currentHealth;

        realHealth = slider.value; // keep synced

        if (currentHealth == 0)
        {
            TriggerDeathCutscene();
        }
    }

    public void TakeDamage(int damage)
    {
        SetHealth(currentHealth - damage);
    }

    public void Heal(int amount)
    {
        SetHealth(currentHealth + amount);
    }

    void TriggerDeathCutscene()
    {
        Debug.Log("Player has died!");

        if (deathCutscene != null)
            deathCutscene.SetActive(true);

        Invoke(nameof(LoadGameOverScene), 3f);
    }

    void LoadGameOverScene()
    {
        SceneManager.LoadScene("GameOver");
    }

    void Die()
    {
        SceneManager.LoadScene(sceneToLoad);// appers the cutscene
        Debug.Log("Player has died!");
    }
}

