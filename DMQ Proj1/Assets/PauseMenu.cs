using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public int MainMenuIndex = 0;

    public void QuitToMainMenu()
    {
        SceneManager.LoadScene(MainMenuIndex);
    }

    public void UnPause()
    {
        Singleton<GameState.GameStateManager>.Instance.InvokeResume();
    }
}
