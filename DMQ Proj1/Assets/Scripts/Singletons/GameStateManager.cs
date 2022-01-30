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
        public static event System.EventHandler<CSEventArgs.StateContext_EventArgs> OnGameStateChanged;

        public void InvokePause()
        {
            StateContext c = new StateContext(GameState.Paused);
            OnGameStateChanged?.Invoke(this, new CSEventArgs.StateContext_EventArgs(c));

        }

        public void InvokeResume()
        {
            StateContext c = new StateContext(GameState.Gameplay);
            OnGameStateChanged?.Invoke(this, new CSEventArgs.StateContext_EventArgs(c));        
        }

    }

}