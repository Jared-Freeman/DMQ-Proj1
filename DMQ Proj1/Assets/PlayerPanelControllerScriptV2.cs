using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPanelControllerScriptV2 : MonoBehaviour
{
    public List<PlayerPanelScriptV2> PlayerPanels = new List<PlayerPanelScriptV2>();
    public List<GameObject> PlayerPanelGOs = new List<GameObject>();
    int currentPlayerIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
        DEBUG_PlayerSpawner.OnPlayerAgentsInstantiated += DEBUG_PlayerSpawner_OnPlayerAgentsInstantiated;
    }

    private void DEBUG_PlayerSpawner_OnPlayerAgentsInstantiated(object sender, CSEventArgs.GameObjectListEventArgs e)
    {
        Debug.Log("instance happened");
        if(currentPlayerIndex < 4)
        {
            foreach (var p in e.gameObjects)
            {
                if (PlayerPanelGOs[currentPlayerIndex] != null)
                {
                    PlayerPanels[currentPlayerIndex].player = new PlayerPanelScriptV2.UIPlayerInfo(p);
                    PlayerPanelGOs[currentPlayerIndex].SetActive(true);
                    currentPlayerIndex++;
                    Debug.Log("This occured as well!");
                }
                else
                {
                    Debug.LogError("Player Panel " + currentPlayerIndex + " is not set!");
                }
            }
        }
    }
}
