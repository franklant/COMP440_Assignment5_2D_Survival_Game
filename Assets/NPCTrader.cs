using UnityEngine;

public class NPCTrader : MonoBehaviour
{
    // Drag your new 'TradingPanel' GameObject here in the Inspector
    public GameObject tradingPanel;

    // This is a simple way to detect interaction
    // Your NPC needs a Collider for this to work
    private void OnMouseDown()
    {
        // Toggle the trading panel's visibility
        if (tradingPanel != null)
        {
            tradingPanel.SetActive(!tradingPanel.activeSelf);
        }
    }
}