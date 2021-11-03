using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]



public class Freeman_Movement_Proto_1 : MonoBehaviour
{

    [SerializeField]
    private float axis_x;
    [SerializeField]
    private float axis_y;

    private Rigidbody attached_rb;


    // Start is called before the first frame update
    void Start()
    {
        attached_rb = GetComponent<Rigidbody>();
        if (attached_rb == null) Debug.LogError("Rigidbody not attached to movement system!");
    }

    // Update is called once per frame
    void Update()
    {
        GatherInput();
        DetermineMovementStates();
        ApplyMovement();
    }

    private void ApplyMovement()
    {
        float horizontal_force = 5f;
        attached_rb.AddForce(new Vector3(axis_x * horizontal_force, 0, axis_y * horizontal_force));
    }

    private void DetermineMovementStates()
    {
        //throw new NotImplementedException();
    }

    private void GatherInput()
    {
        axis_x = Input.GetAxisRaw("Horizontal");
        axis_y = Input.GetAxisRaw("Vertical");
    }
}
