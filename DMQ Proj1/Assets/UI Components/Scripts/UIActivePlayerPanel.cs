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
    private int currentPlayerCount = 0;

    //properties are fancy Fields
    #region Properties 
    #endregion
    // Containers such as structs, or maybe enum declarations
    #region Helpers 
    public class UIPlayerInfo
    {
        public Actor_Player _Player;
        public Inventory_Player _Inventory;
        public int EquipedWeaponIndex = 0;

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

    public List<UIPlayerPanel> ListPlayerPanels = new List<UIPlayerPanel>();
    public List<UIPlayerInfo> Listplayers = new List<UIPlayerInfo>();
    private int playerIndex;
    private int frames = 0;
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
        currentPlayerCount++;
        activateNewPlayerPanel(currentPlayerCount);
        ListPlayerPanels[0].swapWeapon();
        Debug.Log("Event received");
    }

    // Update is called once per frame
    void Update()
    {
        ///<summary>
        ///Updates every 10 frames
        /// </summary>
        frames++;
        if(frames % 10 == 0)
        {
            for(int i = 0; i < Listplayers.Count && i < ListPlayerPanels.Count; i++)
            {
                ///<summary>
                ///Sets character class text
                /// </summary>
                ListPlayerPanels[i].setCharacterClassName(Listplayers[i]._Player.Class.ClassName);

                ///<summary>
                ///Swaps active weapon icon
                /// </summary>
                if(Listplayers[i]._Inventory.GetInfo().EquippedWeaponIndex > -1 
                    && Listplayers[i].EquipedWeaponIndex != Listplayers[i]._Inventory.GetInfo().EquippedWeaponIndex)
                {
                    ListPlayerPanels[i].setActiveWeaponIndex(Listplayers[i]._Inventory.GetInfo().EquippedWeaponIndex);
                    ListPlayerPanels[i].swapWeapon();
                }

                ///<summary>
                ///Sets UI weapon slots
                /// </summary>
                if(Listplayers[i]._Inventory.getWeaponSlots().Count != ListPlayerPanels[i].currentWeaponCount)
                {
                    ListPlayerPanels[i].currentWeaponCount = Listplayers[i]._Inventory.getWeaponSlots().Count;
                    foreach(var j in Listplayers[i]._Inventory.getWeaponSlots())
                    {
                        //Get weapon info here
                    }
                }
                
                ///<summary>
                ///Updates weapon one cool downs
                /// </summary>
                //function here

                ///<summary>
                ///Updates weapon two cool downs
                /// </summary>
                //function here

                ///<summary>
                ///Set item slot one
                /// </summary>
                //function here

                ///<summary>
                ///Update item one cool down
                /// </summary>
                //function here

                ///<summary>
                ///Set item slot two
                /// </summary>
                //function here

                ///<summary>
                ///Update item two cool down
                /// </summary>
                //function here
            }
        }
    }

    void OnDestroy()
    {
        DEBUG_PlayerSpawner.OnPlayerAgentsInstantiated -= HandlePlayerAgentInstantiation;
    }

    void activateNewPlayerPanel(int i)
    {
        ListPlayerPanels[i].activePlayerPanel();
    }
}
