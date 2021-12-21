using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DebugApplyRandomImpulse : MonoBehaviour
{
    
    PlayerControls controls;
    Rigidbody RB;
    public float ForceAmount = 10f;
    private void Awake()
    {
        controls = new PlayerControls();
        if (controls == null) Destroy(this);

        controls.MouseAndKeyboard.SpecialAction.performed += ctx => SpecialActionEvent();
        controls.Gamepad.SpecialAction.performed += ctx => SpecialActionEvent();
    }
    void Start()
    {
        RB = GetComponent<Rigidbody>();

    }
    private void OnEnable()
    {
        controls.Enable();
    }
    private void OnDisable()
    {
        controls.Disable();
    }
    void SpecialActionEvent()
    {
        Vector3 RandomDirection = Vector3.zero;

        RandomDirection.x = Random.Range(-1, 1);
        RandomDirection.y = Random.Range(-1, 1);
        RandomDirection.z = Random.Range(-1, 1);

        RB.AddForce(RandomDirection.normalized * ForceAmount * RB.mass, ForceMode.Impulse);
    }

}
