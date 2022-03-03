using CSEventArgs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIActivePlayerPanel : MonoBehaviour
{
    #region Static Methods 
    #endregion

    #region Events 
    #endregion

    #region Members
    //fields go here



    //properties are fancy Fields
    #region Properties 
    #endregion
    // Containers such as structs, or maybe enum declarations
    #region Helpers 
    public struct UIPlayerInfo
    {
        public Actor_Player _Player;
        public Inventory_Player _Inventory;

        public UIPlayerInfo(GameObject p)
        {
            _Player = p.GetComponent<Actor_Player>();
            _Inventory = p.GetComponent<Inventory_Player>();
        }
    }
    #endregion
    #endregion

    //Start(), Awake(), OnEnable(), +helper methods...
    #region Initialization 

    #endregion

    #region Event Handlers 

    #endregion

    #region Event Subscriptions
    #endregion
    #region structs

    #endregion
    public List<UIPlayerPanel> ListPlayerPanels = new List<UIPlayerPanel>();
    public List<UIPlayerInfo> Listplayers = new List<UIPlayerInfo>();
    private int playerIndex;
    private Canvas canvas;

    private void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        var g = GetComponentsInChildren<UIPlayerPanel>();
        foreach (var p in g)
        {
            if (p != null)
            {
                ListPlayerPanels.Add(p);
            }
        }
        DEBUG_PlayerSpawner.OnPlayerAgentsInstantiated += HandlePlayerAgentInstantiation;
    }

    private void HandlePlayerAgentInstantiation(object sender, GameObjectListEventArgs e)
    {
        foreach(var p in e.gameObjects)
        {
            Listplayers.Add(new UIPlayerInfo(p));
        }
        activatePlayerPanel();
        Debug.Log("Event received");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDestroy()
    {
        DEBUG_PlayerSpawner.OnPlayerAgentsInstantiated -= HandlePlayerAgentInstantiation;
    }

    void activatePlayerPanel()
    {

    }
}
