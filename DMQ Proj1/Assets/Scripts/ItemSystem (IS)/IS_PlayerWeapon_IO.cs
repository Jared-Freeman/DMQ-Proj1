﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Allows players to transmit IO to CurrentWeapon from a PlayerInventory instance.
/// Useful primarily for dispatching attacks
/// </summary>
[RequireComponent(typeof(PlayerInputHost))]
[RequireComponent(typeof(Inventory_Player))]
[RequireComponent(typeof(Actor))]
public class IS_PlayerWeapon_IO : MonoBehaviour
{
    #region Members

    public bool FLAG_Debug = false;

    protected Inventory_Player _Inv;
    public PlayerInputHost InputHost { get; protected set; }
    public PlayerInput _Input { get { return InputHost.CurrentPlayerInput; } }
    protected PlayerControls _Controls;
    protected Actor _Actor;

    protected Vector2 AimDirection = Vector2.zero;

    [SerializeField] Transform AttackContextInitialPosition;
    [SerializeField] float AttackContextInitialPositionForwardOffset = 1.25f;

    protected StateInfo _Info;

    public struct StateInfo
    {
        public bool AttackButtonHeld;
    }
    #endregion


    protected void Awake()
    {
        _Info.AttackButtonHeld = false;

           _Inv = gameObject.GetComponent<Inventory_Player>();
        if(_Inv == null)
        {
            Debug.LogError(ToString() + ": No Player Inventory instance found! Destroying.");
            Destroy(this);
        }

        _Actor = gameObject.GetComponent<Actor>();
        if(_Actor == null)
        {
            Debug.LogError(ToString() + ": No Actor instance found! Destroying.");
            Destroy(this);
        }

        InputHost = gameObject.GetComponent<PlayerInputHost>();
        if(InputHost == null)
        {
            Debug.LogError(ToString() + ": No Input Host instance found! Destroying.");
            Destroy(this);
        }

        InitInput();
    }


    /// <summary>
    /// This function is called in Awake(), and creates _Controls + registers all the events that may occur due to player input
    /// </summary>
    private void InitInput()
    {
        InputHost.OnInputChanged += InputHost_OnInputChanged;

        //_Input = GetComponent<PlayerInput>();
        //if (_Input == null)
        //{
        //    Debug.LogError(ToString() + ": No PlayerInput component found! Destroying.");
        //    Destroy(this);
        //}

        _Controls = new PlayerControls(); //TODO: Maybe optimize or fix

    }//end InitInput()

    private void InputHost_OnInputChanged(object sender, CSEventArgs.PlayerInputEventArgs e)
    {
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
                        _Info.AttackButtonHeld = true;
                        if (TryInvokeAttack()) AttackEvent();
                    }
                    else if (ctx.action.name == _Controls.MouseAndKeyboard.Aim.name)
                    {
                        if (Camera.main == null) return;

                        Vector2 In = ctx.ReadValue<Vector2>();

                        //improved aiming using raycast to plane
                        //TODO: Consider a "raycast to model" approach; consider modifying the CursorPlane to be inline with the projectile spawn height
                        if (Camera.main.GetComponent<Topdown_Multitracking_Camera_Rig>() != null)
                        {

                            //TODO: Consider CamDistanceCurrent improvement. We need a distance from camera to EACH PLAYER, projected onto "2d world plane." 
                            //For now this should be a fairly strong approximation (but maybe could be broken)
                            Plane CursorPlane = new Plane(Vector3.up, Camera.main.GetComponent<Topdown_Multitracking_Camera_Rig>().CamDistanceCurrent);

                            Ray RayToCursorPlane = Camera.main.ScreenPointToRay(new Vector3(In.x, In.y, 0));


                            Vector3 Hit = Vector3.zero;
                            if (CursorPlane.Raycast(RayToCursorPlane, out float Point))
                            {
                                Hit = RayToCursorPlane.GetPoint(Point);
                            }

                            //TOOD: Consider this for relaying info to other subsystems
                            //At this point, Hit == the point on plane where player clicked. Could be useful??

                            Vector3 V = Vector3.ProjectOnPlane((Hit - transform.position), Vector3.up);

                            AimDirection = (new Vector2(V.x, V.z)).normalized;
                        }
                        else
                        {
                            Vector3 V = Camera.main.WorldToScreenPoint(transform.position);
                            //Vector3 V = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, ShootableProjectileHeightOffset, 0)); //hmm 

                            AimDirection = (In - new Vector2(V.x, V.y)).normalized;
                        }
                    }

                }


                else if (ctx.canceled)
                {
                    //MnK
                    if (ctx.action.name == _Controls.MouseAndKeyboard.Attack.name)
                    {
                        _Info.AttackButtonHeld = false;
                    }
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
                        _Info.AttackButtonHeld = true;
                        if (TryInvokeAttack()) AttackEvent();
                    }
                    else if (ctx.action.name == _Controls.Gamepad.Aim.name) AimDirection = ctx.ReadValue<Vector2>().normalized;

                }

                else if (ctx.canceled)
                {
                    //Gamepad
                    if (ctx.action.name == _Controls.Gamepad.Attack.name)
                    {
                        _Info.AttackButtonHeld = false;
                    }
                }
            }
        };


    }

    /// <summary>
    /// Generic attack attempt
    /// </summary>
    /// <returns>True, if attack succeeds</returns>
    private bool TryInvokeAttack()
    {
        if(_Inv.CurrentWeapon != null)
        {
            if (!_Inv.CurrentWeapon.CanAttack) return false; //potentially avoids expense of creating a new attack context

            var aimDir3 = new Vector3(AimDirection.x, 0, AimDirection.y);
            var ctx = new Utils.AttackContext
            {
                _InitialDirection = aimDir3,
                _InitialPosition = AttackContextInitialPosition.position + aimDir3.normalized * AttackContextInitialPositionForwardOffset,
                _InitialGameObject = gameObject,

                _TargetGameObject = null,
                _TargetDirection = Vector3.zero,
                _TargetPosition = AttackContextInitialPosition.position + aimDir3.normalized * AttackContextInitialPositionForwardOffset,

                _Owner = _Actor,
                _Team = _Actor._Team
            };

            return _Inv.CurrentWeapon.InvokeAttack(ctx);
        }
        if (FLAG_Debug) Debug.Log("Attack dispatched, but attempt failed.");
        return false;
    }

    private void AttackEvent()
    {
        if (FLAG_Debug) Debug.Log("AttackEvent!");
    }

    protected void Update()
    {
        if(_Info.AttackButtonHeld)
        {
            TryInvokeAttack();
        }
    }
}
