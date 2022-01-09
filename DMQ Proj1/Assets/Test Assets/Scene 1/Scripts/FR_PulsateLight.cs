using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FR_PulsateLight : MonoBehaviour
{
    Light AttachedLight;

    public AnimationCurve IntensityCurve;
    public AnimationCurve RangeCurve;

    public bool RandomStart = true;


    [Min(0f)]
    public float IntensityMultiplier = 1f;
    [Min(0f)]
    public float RangeMultiplier = 1f;

    [Min(0.01f)]
    public float TimeScale;

    private float CurAnimationT = 0f;

    private float DefaultRange, DefaultIntensity;

    private void Awake()
    {
        AttachedLight = GetComponent<Light>();
        if (AttachedLight == null)
        {
            Debug.LogError("No Light detected on gameobject! Destroying script.");
            Destroy(this);
        }

        DefaultRange = AttachedLight.range;
        DefaultIntensity = AttachedLight.intensity;

        if (RandomStart) CurAnimationT = Random.Range(0, 1000f); //magic number sorry
    }

    private void OnEnable()
    {
        StartCoroutine(ContinuePulse());
    }
    private void OnDisable()
    {
        StopCoroutine(ContinuePulse());
    }


    public IEnumerator ContinuePulse()
    {
        while(true)
        {
            CurAnimationT += Time.deltaTime * TimeScale % 1000f; //magic number sorry

            AttachedLight.intensity = DefaultIntensity * IntensityMultiplier * IntensityCurve.Evaluate(CurAnimationT);
            AttachedLight.range = DefaultRange * RangeMultiplier * RangeCurve.Evaluate(CurAnimationT);

            yield return null;
        }
    }
}
