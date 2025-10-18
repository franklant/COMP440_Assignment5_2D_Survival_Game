using System;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LogicManagerScript : MonoBehaviour
{
    public GameObject globalLight;  // The "Sun" or our main light source
    private Light2D lightSource;    // The actual light source to control the intensity
    public GameObject enemy;
    public GameObject spawnLocation;
    private bool spawnActive;
    public float minutesInADay = 5; // How many minutes make a full day
    public float currentTime = 0;   // Current time
    private float previousTime = 0; // Prevous time passed
    public float daysElapsed = 0;   // Number of days passed
    private float fullDay;          // How much time goes into a day
    private float sunDirection = 1; // After a certain time, the sun goes down
    public float temperature = 20;

    // Keeping track of the time of day and terminology
    public bool isMorning, isNoon, isNight = false;
    private enum dayStates
    {
        // Full day: 5 minutes (300 second); 24 hours in a full day.
        // 300 / 24 = 12.5 seconds per hour
        // 6:00 AM  = 12.5 x 6      = 75 seconds
        // 12:00 PM = 12.5 x 12     = 150 seconds
        // 6:00 PM  = 12.5 x 18     = 225 seconds

        
        MORNING = 75,   // Morning range starting at 75 t0 150
        NOON = 150,     // Noon range starting at 150 to 225
        NIGHT = 225     // Night range starting at 225 to 300
    };



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fullDay = minutesInADay * 60;
        lightSource = globalLight.GetComponent<Light2D>();

        lightSource.intensity = 0;  // Set the world to Night
    }

    // Update is called once per frame
    void Update()
    {
        nightAndDayCycle();

        // day states
        if (currentTime >= (float)dayStates.MORNING && currentTime < (float)dayStates.NOON)
        {
            isMorning = true;   // it is morning time
            isNoon = false;
            isNight = false;
        }
        else if (currentTime >= (float)dayStates.NOON && currentTime < (float)dayStates.NIGHT)
        {
            isMorning = false;
            isNoon = true;      // it is noon time
            isNight = false;
        }
        else if (currentTime >= (float)dayStates.NIGHT || currentTime <= (float)dayStates.MORNING)
        {
            isMorning = false;
            isNoon = false;
            isNight = true;     // it is night time
        }
        else
        {
            isMorning = false;
            isNoon = false;
            isNight = false;
        }

        // take the difference of the night time and current time to get a precise moment for when the day state changed
        // ensures that we only spawn enemies once on one frame only
        float diff = (float)dayStates.NIGHT - currentTime;
        Debug.Log("precision: " + diff);
        if (diff > 0.07 && diff <= 0.09)
        {
            spawnEnemies();
        }
    }

    void nightAndDayCycle()     // Logic for the Night and Day cycle
    {
        if (currentTime < fullDay)  // we haven't reached a full day
        {
            currentTime += Time.deltaTime;

            if (Math.Floor(currentTime) - Math.Floor(previousTime) >= 1)    // full second has passed
            {
                // if positive sundirection, intensity rises
                // if negative sundirection, intensity lowers
                lightSource.intensity += (2 / fullDay) * sunDirection;
            }
        }
        else
        {   // Completed a full day. reset
            currentTime = 0;
            sunDirection = 1;
            daysElapsed += 1;
        }

        // start setting the sun
        if (Math.Floor(currentTime) >= 150) // Noon
        {
            sunDirection = -1;
        }

        if (currentTime % ((12.5 / 60) * 30) <= 0.0055) // increment the temperature every 30 minutes
        {
            temperature += 2 * sunDirection;
        }

        previousTime = currentTime;
    }

    void spawnEnemies()
    {
        // get a list of the transforms from each spawn location
        Transform[] locations = spawnLocation.GetComponentsInChildren<Transform>();
        int maxIndex = 11 - 1; // number of locations - 1

        for (int i = 0; i < 5; i++)
        {
            int locationIndex = UnityEngine.Random.Range(0, maxIndex);
            Transform lo = locations[locationIndex];

            if (lo != null)
            {
                Instantiate(enemy, lo.position, lo.rotation);
                locations[locationIndex] = null;    // make the previously selected location no longer available
            }    
        }
    }

    float getCurrentIntensity()
    {
        return lightSource.intensity;
    }
}
