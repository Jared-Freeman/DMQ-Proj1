using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover_MatchTransformPosition : MonoBehaviour
{
    public Transform ReferenceTransform;

    public Vector3 RelativeOffset = Vector3.zero;

    // Update is called once per frame
    void Update()
    {
        if(ReferenceTransform != null)
        {
            transform.position = ReferenceTransform.position;
            if(RelativeOffset.sqrMagnitude > 0)
            {
                transform.position += ReferenceTransform.transform.forward.normalized * RelativeOffset.x
                    + ReferenceTransform.transform.up.normalized * RelativeOffset.y
                    + ReferenceTransform.transform.right.normalized * RelativeOffset.z;
            }
        }
    }
}
