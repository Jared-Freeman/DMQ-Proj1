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
                    if (ctx.action.name == _Controls.MouseAndKeyboard.Pause.name)
                    {
                        JoinPlayer();
                    }

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
                    if (ctx.action.name == _Controls.Gamepad.Pause.name)
                    {
                        JoinPlayer();
                    }
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

    protected void Start()
    {
        InitializeExistingJoinedPlayers();
    }

    private void InitializeExistingJoinedPlayers()
    {
        foreach(var session in Singleton<PlayerDataManager>.Instance.ActivatedPlayerSessions)
        {
            PlayerInputDataRecord record = new PlayerInputDataRecord();


            record._PlayerInput = session.Info._Input;
            record._Controls = _Controls;
            _JoinedPlayers.Add(record);

            var obj = record._PlayerInput;

            //set up action map
            if (obj.currentControlScheme == _Controls.MouseAndKeyboardScheme.name)
            {
                obj.SwitchCurrentActionMap(_Controls.MouseAndKeyboardScheme.name);
            }
            else if (obj.currentControlScheme == _Controls.GamepadScheme.name)
            {
                obj.SwitchCurrentActionMap(_Controls.GamepadScheme.name);
            }

            //event subscriptions
            obj.onActionTriggered += record.Obj_onActionTriggered;
            record.OnPlayerActivate += Record_OnPlayerActivate;

            if (s_FLAG_DEBUG) Debug.Log("Linked player successfully.");
        }
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


        //set up action map
        if (obj.currentControlScheme == _Controls.MouseAndKeyboardScheme.name)
        {
            obj.SwitchCurrentActionMap(_Controls.MouseAndKeyboardScheme.name);
        }
        else if (obj.currentControlScheme == _Controls.GamepadScheme.name)
        {
            obj.SwitchCurrentActionMap(_Controls.GamepadScheme.name);
        }

        //event subscriptions
        obj.onActionTriggered += record.Obj_onActionTriggered;
        record.OnPlayerActivate += Record_OnPlayerActivate;

        if(s_FLAG_DEBUG) Debug.Log("Linked player successfully.");
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
        //if (s_FLAG_DEBUG) Debug.Log("Player " + e._PlayerInput.gameObject.ToString() + " is attempting to activate!");

        Singleton<PlayerDataManager>.Instance.ActivatePlayer(e._PlayerInput);
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
