using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// translates this object along y axis based on curve
/// </summary>
public class Mover_BobUpAndDown : MonoBehaviour
{
    #region Members
    //flags
    public bool RandomStart = true;

    //refs
    public AnimationCurve HeightCurve;

    //settings
    [Min(0f)]
    public float HeightMultiplier = 1f; // Adjusts amplitude of input curve

    [Min(0.01f)]
    public float TimeScale;

    //state vars
    private float CurrentHeight = 0f; // [-max, +max]
    private float CurAnimationT = 0f;
    private Vector3 DefaultLocalPosition;
    #endregion


    private void Awake()
    {
        if (RandomStart) CurAnimationT = Random.Range(0, 1000f); //magic number sorry

        DefaultLocalPosition = transform.localPosition;
    }


    private void OnEnable()
    {
        StartCoroutine(ContinueBob());
    }
    private void OnDisable()
    {
        StopCoroutine(ContinueBob());
    }


    public IEnumerator ContinueBob()
    {
        Vector3 OffsetVector = Vector3.zero;
        while (true)
        {
            CurAnimationT += Time.deltaTime * TimeScale % 1000f; //magic number sorry

            CurrentHeight = HeightMultiplier * HeightCurve.Evaluate(CurAnimationT);

            OffsetVector.y = CurrentHeight;

            transform.localPosition = DefaultLocalPosition + OffsetVector;

            yield return null;
        }
    }
}
