using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverLogic : MonoBehaviour
{
    public TMP_Text timerText;
    private float timer = 10.5f;

    bool playerWasSpawned = false;

    public string sceneToLoad;
    public string mainMenuSceneToLoad;

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name == sceneToLoad)
        {
            timerText.gameObject.SetActive(true);
            timer -= Time.deltaTime;
            timer = Mathf.Max(timer, 0);
            timerText.text = "Return to Main Menu\n" + Mathf.FloorToInt(timer).ToString();
            if(timer <= 0)
            {
                SceneManager.LoadScene(mainMenuSceneToLoad);

            }
        }
        else
        {

            if (!playerWasSpawned && Singleton<Topdown_Multitracking_Camera_Rig>.Instance.TargetList.Count > 0)
                playerWasSpawned = true;

            else if (playerWasSpawned && Singleton<Topdown_Multitracking_Camera_Rig>.Instance.TargetList.Count == 0)
            {
                Debug.Log("Game Over");
                if (sceneToLoad != null)
                    SceneManager.LoadScene(sceneToLoad);
            }
        }
    }
}
