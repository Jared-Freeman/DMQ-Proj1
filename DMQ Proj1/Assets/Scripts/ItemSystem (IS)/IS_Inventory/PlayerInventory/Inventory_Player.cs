using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Player inventory instance. Grants players agency over picking up items using player input
/// </summary>
public class Inventory_Player : ItemSystem.IS_InventoryBase
{
    PlayerInput Input;
    PlayerControls controls;

    private void Awake()
    {
        InitInput();
    }

    // This function is called in Awake(), and creates controls 
    // + registers all the events that may occur due to player input
    private void InitInput()
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
                    //MnK
                    //if (ctx.action.name == controls.MouseAndKeyboard.Movement.name) InputMap = ctx.ReadValue<Vector2>();

                }


                else if (ctx.canceled)
                {
                    //MnK
                    //if (ctx.action.name == controls.MouseAndKeyboard.Movement.name) InputMap = Vector2.zero;
                }
            }



            ////GAMEPAD EVENTS REGISTER //////////////////////////////////////
            else if (ctx.action.actionMap.name == controls.GamepadScheme.name)
            {
                if (ctx.performed)
                {
                    //Gamepad
                    //if (ctx.action.name == controls.Gamepad.Movement.name) InputMap = ctx.ReadValue<Vector2>();


                }

                else if (ctx.canceled)
                {
                    //Gamepad
                    //if (ctx.action.name == controls.Gamepad.Movement.name) InputMap = Vector2.zero;
                }
            }
        };


    }//end InitInput()


}
