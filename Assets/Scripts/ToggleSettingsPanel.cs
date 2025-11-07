using UnityEngine;
using UnityEngine.UI;

public class ToggleSettingsPanel : MonoBehaviour
{
    [Header("Assign in Inspector")]
    public GameObject targetPanel;  // The GameObject you want to hide/show
    public Button settingsButton;   // The Settings button
    public Button quitButton;       // The Quit button

    void Start()
    {
        // Hide the panel at the start
        if (targetPanel != null)
            targetPanel.SetActive(false);

        // Listen for Settings button click
        if (settingsButton != null)
            settingsButton.onClick.AddListener(TogglePanel);

        // Listen for Quit button click
        if (quitButton != null)
            quitButton.onClick.AddListener(HidePanel);
    }

    void TogglePanel()
    {
        // Toggle the panel's visibility
        if (targetPanel != null)
            targetPanel.SetActive(!targetPanel.activeSelf);
    }

    void HidePanel()
    {
        // Hide the panel when Quit is pressed
        if (targetPanel != null)
            targetPanel.SetActive(false);

        Debug.Log("Quit button pressed — panel hidden.");
    }
}
