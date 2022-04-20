using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MenuScripts
{
    public class Menu_PauseMenuEventProxy : MonoBehaviour
    {
        void Start()
        {
            GameState.GameStateManager.OnGameStateBegin += GameStateManager_OnGameStateChanged;
        }

        void OnDestroy()
        {
            GameState.GameStateManager.OnGameStateBegin -= GameStateManager_OnGameStateChanged;
        }

        private void GameStateManager_OnGameStateChanged(object sender, CSEventArgs.StateContext_EventArgs e)
        {
            if (e.ctx.CurrentState == GameState.GameState.Paused)
            {
                gameObject.SetActive(true);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}