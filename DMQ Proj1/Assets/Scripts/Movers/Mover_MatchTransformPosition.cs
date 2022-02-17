using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover_MatchTransformPosition : MonoBehaviour
{
    public Transform ReferenceTransform;

    // Update is called once per frame
    void Update()
    {
        if(ReferenceTransform != null)
        {
            transform.position = ReferenceTransform.position;
        }
    }
}
