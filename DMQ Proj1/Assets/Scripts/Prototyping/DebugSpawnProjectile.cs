using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script will shoot a projectile in the direction of movement input
public class DebugSpawnProjectile : MonoBehaviour
{
    PlayerControls controls;
    Vector2 InputMap = Vector2.zero;

    #region init
    private void Awake()
    {
        controls = new PlayerControls();
        if (controls == null) Destroy(this);

        //MnK
        controls.MouseAndKeyboard.Attack.performed += ctx => AttackActionEvent();
        controls.MouseAndKeyboard.Movement.performed += ctx => InputMap = ctx.ReadValue<Vector2>();

        //Gamepad
        controls.Gamepad.Attack.performed += ctx => AttackActionEvent();
        controls.Gamepad.Movement.performed += ctx => InputMap = ctx.ReadValue<Vector2>();
    }
    void Start()
    {
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

    void AttackActionEvent()
    {

    }
}
