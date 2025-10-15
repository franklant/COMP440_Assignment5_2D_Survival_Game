using System;
using UnityEngine;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    public GameObject dayText;
    public GameObject hourText;
    public LogicManagerScript logicScript;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        logicScript = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicManagerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        // daysElapsed starts at 0, so the current day would be that plus 1
        dayText.GetComponent<Text>().text = "Day: " + (logicScript.daysElapsed + 1);

        // the current hour is the current time divided by the number of seconds within an hour.
        hourText.GetComponent<Text>().text = "Hour: " + Math.Floor(logicScript.currentTime / 12.5);
    }
}
