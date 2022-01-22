using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Allows players to transmit IO to CurrentWeapon from a PlayerInventory instance.
/// Useful primarily for dispatching attacks
/// </summary>
public class IS_PlayerWeapon_IO : MonoBehaviour
{
    #region Members

    protected Inventory_Player _Inv;
    protected PlayerInput _Input;
    protected PlayerControls _Controls;

    #endregion


    protected void Awake()
    {
        _Inv = gameObject.GetComponent<Inventory_Player>();
        if(_Inv == null)
        {
            Debug.LogError(ToString() + ": No Player Inventory instance found! Destroying.");
            Destroy(this);
        }

        InitInput();
    }


    /// <summary>
    /// This function is called in Awake(), and creates _Controls + registers all the events that may occur due to player input
    /// </summary>
    private void InitInput()
    {
        _Input = GetComponent<PlayerInput>();
        if (_Input == null)
        {
            Debug.LogError(ToString() + ": No PlayerInput component found! Destroying.");
            Destroy(this);
        }

        _Controls = new PlayerControls(); //TODO: Maybe optimize or fix

        //set up action map
        if (_Input.currentControlScheme == _Controls.MouseAndKeyboardScheme.name)
        {
            _Input.SwitchCurrentActionMap(_Controls.MouseAndKeyboardScheme.name);
        }
        else if (_Input.currentControlScheme == _Controls.GamepadScheme.name)
        {
            _Input.SwitchCurrentActionMap(_Controls.GamepadScheme.name);
        }

        //please someone find a better way to do this (and retain multiplayer functionality)
        _Input.onActionTriggered += ctx =>
        {
            ////MOUSE AND KEYBOARD EVENTS REGISTER //////////////////////////////////////
            if (ctx.action.actionMap.name == _Controls.MouseAndKeyboardScheme.name)
            {
                if (ctx.performed)
                {
                    //MnK
                    if (ctx.action.name == _Controls.MouseAndKeyboard.Attack.name)
                    {
                        TryInvokeAttack();
                    }

                }


                else if (ctx.canceled)
                {
                    //MnK
                    //if (ctx.action.name == _Controls.MouseAndKeyboard.Movement.name) InputMap = Vector2.zero;
                }
            }



            ////GAMEPAD EVENTS REGISTER //////////////////////////////////////
            else if (ctx.action.actionMap.name == _Controls.GamepadScheme.name)
            {
                if (ctx.performed)
                {
                    //Gamepad
                    if (ctx.action.name == _Controls.Gamepad.Attack.name)
                    {
                        TryInvokeAttack();
                    }

                }

                else if (ctx.canceled)
                {
                    //Gamepad
                    //if (ctx.action.name == _Controls.Gamepad.Movement.name) InputMap = Vector2.zero;
                }
            }
        };


    }//end InitInput()

    private void TryInvokeAttack()
    {
        if(_Inv.CurrentWeapon != null)
        {

        }
        throw new NotImplementedException();
    }
}
