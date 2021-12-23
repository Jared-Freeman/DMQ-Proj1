using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugApplyImpulseTowardsTarget : MonoBehaviour
{
    PlayerControls controls;
    Rigidbody RB;
    public float ForceAmount = 30f;
    public Transform Target;

    #region init
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
    #endregion

    void SpecialActionEvent()
    {
        //Debug.Log("Impulse!");
        RB.AddForce((Target.position - transform.position).normalized * ForceAmount * RB.mass, ForceMode.Impulse);
    }
}
