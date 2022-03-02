using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSEventArgs
{
    public class StateContext_EventArgs : System.EventArgs
    {
        public GameState.StateContext ctx;
        public StateContext_EventArgs(GameState.StateContext c)
        {
            ctx = c;
        }
    }
}

namespace GameState
{
    public enum GameState { Gameplay, Paused /*, Loading*/} //probably need to expand later

    /// <summary>
    /// Context of a game state change. Add stuff here that needs to be communicated on a Game State change
    /// </summary>
    /// <remarks>
    /// This is a message packet sent in <see cref="CSEventArgs.StateContext_EventArgs"/> for GameState events.
    /// </remarks>
    [System.Serializable]
    public class StateContext
    {
        public StateContext(GameState curState)
        {
            CurrentState = curState;
        }

        /// <summary>
        /// State the game is currently in
        /// </summary>
        public GameState CurrentState;
    }


    /// <summary>
    /// Contains methods for changing state of the game, such as invoking a pause or resume.
    /// </summary>
    public class GameStateManager : Singleton<GameStateManager>
    {
        #region Events

        public static event System.EventHandler<CSEventArgs.StateContext_EventArgs> OnGameStateChanged;
        public static event System.EventHandler<CSEventArgs.StateContext_EventArgs> OnGameStateEnd;
        public static event System.EventHandler<CSEventArgs.StateContext_EventArgs> OnGameStateBegin;

        #endregion

        #region Members

        /// <summary>
        /// For internal input parsing
        /// </summary>
        private PlayerControls _PlayerControls;
        /// <summary>
        /// The current Game State
        /// </summary>
        public GameState CurrentGameState
        {
            get;
            private set;
        }

        #endregion

        #region Event Handlers

        private void PlayerDataManager_OnPlayerActivated(object sender, PlayerDataManager.PlayerDataSessionEventArgs e)
        {
            e.Data.Info._Input.onActionTriggered += _Input_onActionTriggered;
        }
        private void PlayerDataManager_OnPlayerDeactivated(object sender, PlayerDataManager.PlayerDataSessionEventArgs e)
        {
            e.Data.Info._Input.onActionTriggered -= _Input_onActionTriggered;
        }

        /// <summary>
        /// Input event handler.
        /// </summary>
        /// <param name="ctx">The input context</param>
        private void _Input_onActionTriggered(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
        {
            //Debug.Log("Player used: " + ctx.action.name);

            ////MOUSE AND KEYBOARD EVENTS REGISTER //////////////////////////////////////
            if (ctx.action.actionMap.name == _PlayerControls.MouseAndKeyboardScheme.name)
            {
                if (ctx.performed)
                {
                    //MnK
                    if (ctx.action.name == _PlayerControls.MouseAndKeyboard.Movement.name)
                    {

                    }

                }

                else if (ctx.canceled)
                {
                }
            }


            ////GAMEPAD EVENTS REGISTER //////////////////////////////////////
            else if (ctx.action.actionMap.name == _PlayerControls.GamepadScheme.name)
            {
                if (ctx.performed)
                {
                    //Gamepad
                    if (ctx.action.name == _PlayerControls.Gamepad.Movement.name)
                    {

                    }
                }

                else if (ctx.canceled)
                {
                }
            }
        }

        #endregion

        #region Initialization

        protected override void Awake()
        {
            base.Awake();
            _PlayerControls = new PlayerControls();
        }

        void Start()
        {
            //Listen for pause events. Could filter based on P1 in event handler if need be
            PlayerDataManager.OnPlayerActivated += PlayerDataManager_OnPlayerActivated;
            PlayerDataManager.OnPlayerDeactivated += PlayerDataManager_OnPlayerDeactivated;
        }


        #endregion

        #region Destroy

        void OnDestroy()
        {
            PlayerDataManager.OnPlayerActivated -= PlayerDataManager_OnPlayerActivated;
            PlayerDataManager.OnPlayerActivated -= PlayerDataManager_OnPlayerDeactivated;
        }

        #endregion



        public void InvokePause()
        {
            ChangeGameState(GameState.Paused);
        }

        public void InvokeResume()
        {
            ChangeGameState(GameState.Gameplay);
        }

        private void ChangeGameState(GameState s)
        {
            OnGameStateEnd?.Invoke(this, new CSEventArgs.StateContext_EventArgs(new StateContext(CurrentGameState)));

            GameStateEnd(CurrentGameState);

            CurrentGameState = s;

            GameStateBegin(CurrentGameState);

            OnGameStateBegin?.Invoke(this, new CSEventArgs.StateContext_EventArgs(new StateContext(CurrentGameState)));
            OnGameStateChanged?.Invoke(this, new CSEventArgs.StateContext_EventArgs(new StateContext(CurrentGameState)));
        }

        private void GameStateEnd(GameState s)
        {
            switch (s)
            {
                case GameState.Gameplay:
                    break;
                case GameState.Paused:
                    break;

                default:
                    Debug.LogError("Unrecognized GameState? Does impl exist?");
                    break;
            }
        }
        private void GameStateBegin(GameState s)
        {
            switch (s)
            {
                case GameState.Gameplay:
                    break;
                case GameState.Paused:
                    break;

                default:
                    Debug.LogError("Unrecognized GameState? Does impl exist?");
                    break;
            }
        }
    }

}