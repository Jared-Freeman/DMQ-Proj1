using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public int LoadSceneBuildIndex = 1;

    public void PlayGame ()
    {
        SceneManager.LoadScene(LoadSceneBuildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
