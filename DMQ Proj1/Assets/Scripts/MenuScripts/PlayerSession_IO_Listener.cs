using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

/// <summary>
/// Coordinates Input System with the PlayerData singleton to determine Active players (versus just Join'd players)
/// </summary>
public class PlayerSession_IO_Listener : MonoBehaviour
{
    #region Members

    protected static bool s_FLAG_DEBUG = true;

    [SerializeField] private List<PlayerInputDataRecord> _JoinedPlayers;

    private PlayerControls _Controls;

    #region Properties

    public IReadOnlyCollection<PlayerInputDataRecord> ActivePlayers { get { return _JoinedPlayers.AsReadOnly(); } }

    #endregion

    #region Helpers

    public class PlayerInputEventArgs : System.EventArgs
    {
        public PlayerInput _PlayerInput;
        public PlayerInputEventArgs(PlayerInput p)
        {
            _PlayerInput = p;
        }
    }

    [System.Serializable]
    public class PlayerInputDataRecord
    {
        #region Events

        /// <summary>
        /// Fired when a joined PlayerInput presses the "start" button and attempts to become an active player
        /// </summary>
        public event System.EventHandler<PlayerInputEventArgs> OnPlayerActivate;

        #endregion

        #region Members

        public PlayerInput _PlayerInput;
        public PlayerControls _Controls;

        #endregion

        #region Event Handler Methods

        public void Obj_onActionTriggered(InputAction.CallbackContext ctx)
        {

            ////MOUSE AND KEYBOARD EVENTS REGISTER //////////////////////////////////////
            if (ctx.action.actionMap.name == _Controls.MouseAndKeyboardScheme.name)
            {
                if (ctx.performed)
                {
                    //MnK
                    if (ctx.action.name == _Controls.MouseAndKeyboard.Movement.name) { }

                    else if (ctx.action.name == _Controls.MouseAndKeyboard.Attack.name)
                    {
                        JoinPlayer();
                    }

                    else if (ctx.action.name == _Controls.MouseAndKeyboard.SpecialAction.name) { }

                    else if (ctx.action.name == _Controls.MouseAndKeyboard.Wepon1Equip.name) { }

                    else if (ctx.action.name == _Controls.MouseAndKeyboard.Wepon2Equip.name) { }

                }


                else if (ctx.canceled)
                {
                }
            }

            ////GAMEPAD EVENTS REGISTER //////////////////////////////////////
            else if (ctx.action.actionMap.name == _Controls.GamepadScheme.name)
            {
                if (ctx.performed)
                {
                    //Gamepad
                    if (ctx.action.name == _Controls.Gamepad.Movement.name) { }

                    else if (ctx.action.name == _Controls.Gamepad.Attack.name)
                    {
                        JoinPlayer();
                    }

                    else if (ctx.action.name == _Controls.Gamepad.SpecialAction.name) { }

                    else if (ctx.action.name == _Controls.Gamepad.Wepon1Equip.name) { }

                    else if (ctx.action.name == _Controls.Gamepad.Wepon2Equip.name) { }

                    else if (ctx.action.name == _Controls.Gamepad.Aim.name) { }
                }


                else if (ctx.canceled)
                {
                }
            }
        }

        /// <summary>
        /// Helper method for event handler
        /// </summary>
        private void JoinPlayer()
        {
            OnPlayerActivate?.Invoke(this, new PlayerInputEventArgs(_PlayerInput));
        }

        #endregion

    }

    #endregion

    #endregion

    #region Initialization

    protected void Awake()
    {
        _JoinedPlayers = new List<PlayerInputDataRecord>();
        _Controls = new PlayerControls();
    }

    #endregion

    #region Init Event Subscriptions

    void OnEnable()
    {
        Singleton<PlayerManager_Proxy>.Instance.InputManager.onPlayerJoined += InputManager_onPlayerJoined;
        Singleton<PlayerManager_Proxy>.Instance.InputManager.onPlayerLeft += InputManager_onPlayerLeft;
    }
    void OnDisable()
    {        
        //Singleton<PlayerManager_Proxy>.Instance.InputManager.onPlayerJoined -= InputManager_onPlayerJoined;
        //Singleton<PlayerManager_Proxy>.Instance.InputManager.onPlayerLeft -= InputManager_onPlayerLeft;
    }
    #endregion

    #region Event Handlers

    private void InputManager_onPlayerJoined(UnityEngine.InputSystem.PlayerInput obj)
    {
        PlayerInputDataRecord record = new PlayerInputDataRecord();

        record._PlayerInput = obj;
        record._Controls = _Controls;

        _JoinedPlayers.Add(record);


        obj.onActionTriggered += record.Obj_onActionTriggered;
        record.OnPlayerActivate += Record_OnPlayerActivate;
    }

    private void InputManager_onPlayerLeft(UnityEngine.InputSystem.PlayerInput obj)
    {
        var record = FindRecord(obj);

        if(record != null)
        {
            _JoinedPlayers.Remove(record);
            record._PlayerInput.onActionTriggered -= record.Obj_onActionTriggered;
            record.OnPlayerActivate -= Record_OnPlayerActivate;
        }
    }

    private void Record_OnPlayerActivate(object sender, PlayerInputEventArgs e)
    {
        if(s_FLAG_DEBUG) Debug.Log("Player " + e._PlayerInput.gameObject.ToString() + " is attempting to activate!");
    }


    #endregion



    #region Utility Methods

    PlayerInputDataRecord FindRecord(PlayerInput obj)
    {
        foreach (var r in _JoinedPlayers)
        {
            if (r._PlayerInput = obj) return r;
        }
        return null;
    }

    #endregion
}
