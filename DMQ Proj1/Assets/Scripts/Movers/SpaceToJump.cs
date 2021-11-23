using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class SpaceToJump : MonoBehaviour
{
    public float ImpulseForce = 5;

    private Rigidbody RB;

    // Start is called before the first frame update
    void Start()
    {
        RB = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Jump"))
        {
            RB.AddForce(Vector3.up * ImpulseForce * RB.mass + transform.forward * ImpulseForce * RB.mass, ForceMode.Impulse);
        }
    }

}
