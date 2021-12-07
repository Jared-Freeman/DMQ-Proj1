using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchScript : MonoBehaviour
{
    Light torch;
    float baseIntensity = 2;
    float baseRange = 3;
    public float max = 3.0f;
    public float min = 1.0f;
    float t = 0;
    void Start()
    {
        torch = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;
        torch.intensity = baseIntensity + Mathf.Lerp(min, max, t);
        torch.range = baseRange + Mathf.Lerp(min, max, t);
        if (t > 1.0)
        {
            float temp = max;
            max = min;
            min = temp;
            t = 0.0f;
        }
    }
}
