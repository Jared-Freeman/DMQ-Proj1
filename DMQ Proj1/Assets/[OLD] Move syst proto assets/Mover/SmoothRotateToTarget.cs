using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothRotateToTarget : MonoBehaviour
{
    public GameObject target;

    [Space(10f)]

    [Tooltip("Absolute maximum difference between target's Euler angles and this object's Euler angles")]
    [Range(0,179)]
    public float LeashAngle = 20f;

    [Space(10f)]

    [Header("Angle Flags")]
    public bool LockEulerX = false;
    public bool LockEulerY = false;
    public bool LockEulerZ = false;

    [Space(30f)]

    [SerializeField]
    private Vector3 RotationVector = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        if(target == null) { target = gameObject; }
    }

    // Update is called once per frame
    void Update()
    {
        RotationVector = Vector3.RotateTowards(transform.rotation.eulerAngles, target.transform.rotation.eulerAngles, 100 * Time.deltaTime, 999f);

        if (!LockEulerX)
        {
            //Vector3 VectorDifference = transform.rotation.eulerAngles - target.transform.rotation.eulerAngles;
            transform.Rotate(new Vector3(RotationVector.x, 0,0));
        }
        if (!LockEulerY)
        {
            //transform.Rotate(Vector3.RotateTowards(transform.rotation.eulerAngles, target.transform.rotation.eulerAngles, 20 * Time.deltaTime, 0f));
            transform.Rotate(new Vector3(0, RotationVector.y, 0));
        }
        if (!LockEulerZ)
        {
            //transform.Rotate(Vector3.RotateTowards(transform.rotation.eulerAngles, target.transform.rotation.eulerAngles, 20 * Time.deltaTime, 0f));
            transform.Rotate(new Vector3(0, 0, RotationVector.z));
        }
    }
}
