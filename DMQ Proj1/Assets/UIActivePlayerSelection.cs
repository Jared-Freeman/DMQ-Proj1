using CSEventArgs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIActivePlayerSelection : MonoBehaviour
{

    public List<GameObject> ListPlayerPanels = new List<GameObject>();
    public List<GameObject> PressStart = new List<GameObject>();
    private int currentPlayerCount = 1;
    private int maxPlayerCount = 4;
    // Start is called before the first frame update
    void Start()
    {
        PlayerDataManager.OnPlayerActivated += PlayerDataManager_OnPlayerActivated;
    }

    private void PlayerDataManager_OnPlayerActivated(object sender, PlayerDataManager.PlayerDataSessionEventArgs e)
    {
        //Need to init for ALL incoming gameobjects
        for (int i = 0; (i < currentPlayerCount) && (currentPlayerCount < maxPlayerCount + 1); i++)
        {
            activateClassSelectPanel(i);

            Debug.Log("Event received");
        }
        currentPlayerCount++;
    }

    void activateClassSelectPanel(int i)
    {
        if (i > -1)
        {
            ListPlayerPanels[i].SetActive(true);
            PressStart[i].SetActive(false);
        }
    }

    void OnDestroy()
    {
        PlayerDataManager.OnPlayerActivated -= PlayerDataManager_OnPlayerActivated;
    }
}
