using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ClassSystem;
using UnityEngine.InputSystem;


public class DEBUG_PlayerManualSpawner : MonoBehaviour
{
    private int currentPlayerIndex = 0;
    public Vector3 playerSpawnOffset;
    public static event System.EventHandler<CSEventArgs.GameObjectListEventArgs> OnPlayerAgentsInstantiated;
    void Start()
    {
        List<GameObject> Glist = new List<GameObject>();
        if (PlayerDataManager.InstanceExists)
        {
            GameObject g;
            for(int i=0;i<PlayerDataManager.Instance.ActivatedPlayerSessions.Count;i++)
            {
                PlayerData_Session r = PlayerDataManager.Instance.ActivatedPlayerSessions[i];
                CharacterClass currentClass = r.Info._CurrentClassPreset;
                if(currentClass != null)
                {
                    g = currentClass?.InstantiatePlayerActor();
                    if (g != null)
                    {
                        InitPlayerActor(g, r.Info._Input);
                    }
                    Glist.Add(g);
                    OnPlayerAgentsInstantiated?.Invoke(this, new CSEventArgs.GameObjectListEventArgs(Glist));
                    currentPlayerIndex++;
                }
            }
        }
        
    }
    void InitPlayerActor(GameObject g, PlayerInput p)
    {
        if(currentPlayerIndex == 0)
            g.transform.position = transform.position;
        else
        {
            //Apply an offset to position
            g.transform.position = transform.position + playerSpawnOffset;
        }

        var h = g.GetComponent<PlayerInputHost>();
        h.CurrentPlayerInput = p;
    }
}
