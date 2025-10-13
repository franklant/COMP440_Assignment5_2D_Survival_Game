using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightSourceScript : MonoBehaviour
{
    public GameObject lightSource;
    private new Light2D light;
    public LogicManagerScript logicScript;  // retreive an instance of the logic script

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        logicScript = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicManagerScript>();

        light = lightSource.GetComponent<Light2D>();
        // light.intensity = 0;    // no light
    }

    // Update is called once per frame
    void Update()
    {
        // check if it's a certain time and change the visibility of the light source
        if (logicScript.isNight == true)
        {
            light.intensity = 1;
        }
        else
        {
            light.intensity = 0;    // no light
        }
    }
}
