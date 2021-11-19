using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightScript : MonoBehaviour
{
    
    public Light spotlight;
    public float baseRange = 120;
    public float baseAngle = 96;
    public float baseIntensity = 10;
    public float maximum = 1.0f;
    public float minimum = -1.0f;
    float t = 0;

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;
        spotlight.intensity = baseIntensity + Mathf.Lerp(minimum, maximum, t);
        spotlight.range = baseRange + (Mathf.Lerp(minimum, maximum, t) * 1);
        spotlight.spotAngle = baseAngle + (Mathf.Lerp(minimum, maximum, t) /5);
        if (t > 1.0)
        {
            float temp = maximum;
            maximum = minimum;
            minimum = temp;
            t = 0.0f;
        }
    }
}
