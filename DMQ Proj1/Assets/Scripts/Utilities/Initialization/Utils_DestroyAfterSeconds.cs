using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This component simply destroys the object it is attached to after the specified duration.
/// </summary>
/// <remarks>
/// Useful for ragdolls and other debris that should be cleaned up
/// </remarks>
public class Utils_DestroyAfterSeconds : MonoBehaviour
{
    public float Duration;

    void Start()
    {
        StartCoroutine(DestroyAfterSeconds(Duration));
    }

    protected IEnumerator DestroyAfterSeconds(float time)
    {
        yield return new WaitForSeconds(time);

        Destroy(gameObject);
    }
}
