using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverLogic : MonoBehaviour
{
    public Topdown_Multitracking_Camera_Rig camera_Rig;

    bool playerWasSpawned = false;

    public string sceneToLoad;

    // Update is called once per frame
    void Update()
    {
        if (!playerWasSpawned && camera_Rig.TargetList.Count > 0)
            playerWasSpawned = true;

        else if(playerWasSpawned && camera_Rig.TargetList.Count == 0)
        {
            if(sceneToLoad != null)
                SceneManager.LoadScene(sceneToLoad);
        }
    }
}
