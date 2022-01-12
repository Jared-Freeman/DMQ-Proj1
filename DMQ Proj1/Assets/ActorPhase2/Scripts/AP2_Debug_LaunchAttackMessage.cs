using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Actor))]
[RequireComponent(typeof(PlayerInput))]
public class AP2_Debug_LaunchAttackMessage : MonoBehaviour
{
    //refs
    public GameObject Target;
    public AP2_ActorAction_AttackTarget AttackAction;

    //refs
    private Actor _Actor;
    private PlayerInput Input;
    private PlayerControls controls;

    private void Awake()
    {
        Input = GetComponent<PlayerInput>();
        if (Input == null) Destroy(this);

        _Actor = GetComponent<Actor>();
        if (_Actor == null) Destroy(this);

        if (Target == null)
        {
            Debug.LogError("DEBUG: No target specified!");
            Destroy(this);
        }

        InitInput();
    }

    void InitInput()
    {
        controls = new PlayerControls();

        //set up action map
        if (Input.currentControlScheme == controls.MouseAndKeyboardScheme.name)
        {
            Input.SwitchCurrentActionMap(controls.MouseAndKeyboardScheme.name);
        }
        else if (Input.currentControlScheme == controls.GamepadScheme.name)
        {
            Input.SwitchCurrentActionMap(controls.GamepadScheme.name);
        }

        //please someone find a better way to do this (and retain multiplayer functionality)
        Input.onActionTriggered += ctx =>
        {
            ////MOUSE AND KEYBOARD EVENTS REGISTER //////////////////////////////////////
            if (ctx.action.actionMap.name == controls.MouseAndKeyboardScheme.name)
            {
                if (ctx.performed)
                {
                //    //MnK
                //    if (ctx.action.name == controls.MouseAndKeyboard.Movement.name) InputMap = ctx.ReadValue<Vector2>();

                //    else if (ctx.action.name == controls.MouseAndKeyboard.Attack.name) AttackEvent();

                //    else if (ctx.action.name == controls.MouseAndKeyboard.SpecialAction.name) SpecialActionEvent();

                    if (ctx.action.name == controls.MouseAndKeyboard.Wepon1Equip.name) PerformDebugAttack();

                    //else if (ctx.action.name == controls.MouseAndKeyboard.Wepon2Equip.name) ChangeWeaponEvent();

                    //else if (ctx.action.name == controls.MouseAndKeyboard.Aim.name)
                    //{
                    //    Vector3 V = Camera.main.WorldToScreenPoint(transform.position);
                    //    AimDirection = (ctx.ReadValue<Vector2>() - new Vector2(V.x, V.y)).normalized;
                    //}
                }


                //else if (ctx.canceled)
                //{
                //    //MnK
                //    if (ctx.action.name == controls.MouseAndKeyboard.Movement.name) InputMap = Vector2.zero;
                //}
            }



            ////GAMEPAD EVENTS REGISTER //////////////////////////////////////
            else if (ctx.action.actionMap.name == controls.GamepadScheme.name)
            {
                if (ctx.performed)
                {
                    ////Gamepad
                    //if (ctx.action.name == controls.Gamepad.Movement.name) InputMap = ctx.ReadValue<Vector2>();

                    //else if (ctx.action.name == controls.Gamepad.Attack.name) AttackEvent();

                    //else if (ctx.action.name == controls.Gamepad.SpecialAction.name) SpecialActionEvent();

                    //else if (ctx.action.name == controls.Gamepad.Wepon1Equip.name) ChangeWeaponEvent();

                    if (ctx.action.name == controls.Gamepad.Wepon2Equip.name) PerformDebugAttack();

                    //else if (ctx.action.name == controls.Gamepad.Aim.name) AimDirection = ctx.ReadValue<Vector2>().normalized;
                }


                //else if (ctx.canceled)
                //{
                //    //Gamepad
                //    if (ctx.action.name == controls.Gamepad.Movement.name) InputMap = Vector2.zero;
                //}
            }
        };
    }

    void PerformDebugAttack()
    {
        if (AttackAction != null)
        {
            //load attack options
            AttackAction.Options.Target = Target;

            //dispatch action
            AttackAction.PerformAction(_Actor);
        }
        else
            Debug.LogError("ActorAction not found!");
    }
}
