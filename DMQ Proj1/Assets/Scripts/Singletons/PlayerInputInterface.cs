using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

namespace Input_DMQ
{
    /// <summary>
    /// Enables organizing input-listening scripts (by player that owns them). 
    /// Input listeners may be further subdivided into InputChannels which may be enabled/disabled when needed.
    /// </summary>
    public class PlayerInputInterface : Singleton<PlayerInputInterface>
    {
        public PlayerInputManager _InputManager;

        public List<PlayerInputContext> _PlayerInputContexts = new List<PlayerInputContext>();
        public IReadOnlyCollection<PlayerInputContext> ActivePlayers
        {
            get
            {
                return _PlayerInputContexts.AsReadOnly();
            }
        }

        //public List<PlayerInput> _PlayerInputs = new List<PlayerInput>();
        //public IReadOnlyCollection<PlayerInput> ActivePlayerInputs
        //{
        //    get
        //    {
        //        return _PlayerInputs.AsReadOnly();
        //    }
        //}

        /// <summary>
        /// Returns input context for the specified PlayerInput. Useful for accessing input channels
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public PlayerInputContext GetContext(PlayerInput player)
        {
            foreach (var pc in _PlayerInputContexts)
            {
                if(pc.Player == player)
                {
                    return pc;
                }
            }

            return null;
        }


        #region Event Subscriptions
        void OnEnable()
        {
            DMQ_PlayerInputManager_Relay.onPlayerInputManagerCreate += DMQ_PlayerInputManager_Relay_onPlayerInputManagerCreate;
            DMQ_PlayerInputManager_Relay.onPlayerInputManagerDestroy += DMQ_PlayerInputManager_Relay_onPlayerInputManagerDestroy;

            if (_InputManager != null)
            {
                _InputManager.onPlayerJoined += _InputManager_onPlayerJoined;
                _InputManager.onPlayerLeft += _InputManager_onPlayerLeft;
            }
        }

        void OnDisable()
        {
            DMQ_PlayerInputManager_Relay.onPlayerInputManagerCreate -= DMQ_PlayerInputManager_Relay_onPlayerInputManagerCreate;
            DMQ_PlayerInputManager_Relay.onPlayerInputManagerDestroy -= DMQ_PlayerInputManager_Relay_onPlayerInputManagerDestroy;

            if(_InputManager != null)
            {
                _InputManager.onPlayerJoined -= _InputManager_onPlayerJoined;
                _InputManager.onPlayerLeft -= _InputManager_onPlayerLeft;

            }
        }

        #endregion

        #region Event Handlers

        private void DMQ_PlayerInputManager_Relay_onPlayerInputManagerDestroy(object sender, CSEventArgs.PlayerInputManager_Args e)
        {
            if(_InputManager == e.Manager)
            {
                _InputManager.onPlayerJoined -= _InputManager_onPlayerJoined;
                _InputManager.onPlayerLeft -= _InputManager_onPlayerLeft;
                _InputManager = null;
            }
            else
            {
                throw new System.Exception("Multiple Input Manager references have been relayed to " + ToString());
            }
        }

        private void DMQ_PlayerInputManager_Relay_onPlayerInputManagerCreate(object sender, CSEventArgs.PlayerInputManager_Args e)
        {
            if (_InputManager == null)
            {
                _InputManager = e.Manager;
                _InputManager.onPlayerJoined += _InputManager_onPlayerJoined;
                _InputManager.onPlayerLeft += _InputManager_onPlayerLeft;
            }
        }

        private void _InputManager_onPlayerJoined(PlayerInput obj)
        {
            _PlayerInputContexts.Add(new PlayerInputContext(obj));
        }
        private void _InputManager_onPlayerLeft(PlayerInput obj)
        {
            _PlayerInputContexts.Remove(new PlayerInputContext(obj));
        }

        #endregion
    }


    /// <summary>
    /// Container for PlayerInput, InputChannel(s), etc.
    /// </summary>
    public class PlayerInputContext
    {
        public PlayerInputContext(PlayerInput input) : base()
        {
            Player = input;
            _InputChannels.Add(new InputChannel("default", Player));
        }

        public PlayerInput Player { get; private set; }

        public List<InputChannel> _InputChannels = new List<InputChannel>();

        public InputChannel SubscribeToChannel(string name, MonoBehaviour subscriber)
        {
            foreach (var c in _InputChannels)
            {
                if (c.ChannelName == name)
                {
                    c.AddSubscriber(subscriber);
                    return c;
                }
            }


            InputChannel newChannel = new InputChannel(name, Player);
            newChannel.AddSubscriber(subscriber);
            return newChannel;
        }
    }

    /// <summary>
    /// Controls when input events are relayed to subscribers. Subscribers may be accessed here. Note that subscribers must still subscribe to the appropriate methods to gain any functionality from them.
    /// </summary>
    public class InputChannel
    {
        #region Events

        public event System.EventHandler<InputAction.CallbackContext> onActionTriggered;
        public event System.EventHandler<PlayerInput> onControlsChanged;
        public event System.EventHandler<PlayerInput> onDeviceLost;
        public event System.EventHandler<PlayerInput> onDeviceRegained;

        #region Helpers
        private void Invoke_onActionTriggered(InputAction.CallbackContext ctx)
        {
            if (ChannelEnabled)
                onActionTriggered?.Invoke(this, ctx);
        }
        private void Invoke_onControlsChanged(PlayerInput inp)
        {
            if (ChannelEnabled)
                onControlsChanged?.Invoke(this, inp);
        }
        private void Invoke_onDeviceLost(PlayerInput inp)
        {
            if (ChannelEnabled)
                onDeviceLost?.Invoke(this, inp);
        }
        private void Invoke_onDeviceRegained(PlayerInput inp)
        {
            if (ChannelEnabled)
                onDeviceRegained?.Invoke(this, inp);
        }
        #endregion

        #endregion

        public List<MonoBehaviour> Subscribers { get; private set; } = new List<MonoBehaviour>();
        public PlayerInput LinkedInput { get; private set;}

        //enabled by default
        public bool ChannelEnabled { get; set; } = true;

        public string ChannelName
        {
            get
            {
                return ChannelName;
            }
            private set
            {
                //filter out white space and uppercase
                string filteredString = string.Concat(value.Where(c => !char.IsWhiteSpace(c)));
                ChannelName = filteredString.ToLower();
            }
        }

        #region Construction/Destruction

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="name">name of channel</param>
        /// <param name="linked_input">PlayerInput reference to listen to input events from</param>
        public InputChannel(string name, PlayerInput linked_input)
        {
            ChannelName = name;
            LinkedInput = linked_input;
        }

        /// <summary>
        /// Dtor
        /// </summary>
        ~InputChannel()
        {
            LinkedInput.onActionTriggered -= Invoke_onActionTriggered;
            LinkedInput.onControlsChanged -= Invoke_onControlsChanged;
            LinkedInput.onDeviceLost -= Invoke_onDeviceLost;
            LinkedInput.onDeviceRegained -= Invoke_onDeviceRegained;
        }

        #endregion

        public void AddSubscriber(MonoBehaviour script)
        {
            Subscribers.Add(script);
        }
        public void RemoveSubscriber(MonoBehaviour script)
        {
            Subscribers.Remove(script);
        }
    }

}

