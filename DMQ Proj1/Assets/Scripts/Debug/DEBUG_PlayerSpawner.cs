using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ClassSystem;
using UnityEngine.InputSystem;

namespace CSEventArgs
{
    public class GameObjectListEventArgs : System.EventArgs
    {
        public List<GameObject> gameObjects;

        public GameObjectListEventArgs(List<GameObject> gos)
        {
            gameObjects = gos;
        }
    }
}

/// <summary>
/// Looks at ActivatedPlayers and spawns the appropriate player agent in
/// </summary>
public class DEBUG_PlayerSpawner : MonoBehaviour
{
    private int _CurrentClassSpawnIndex = 0;
    public bool ForceUseSpawnQueue = true;
    public List<CharacterClass> ClassSpawnQueue = new List<CharacterClass>();

    /// <summary>
    /// Fires when the gameobjects that represent the players are created and initialized. New batches may be fired at any time
    /// </summary>
    public static event System.EventHandler<CSEventArgs.GameObjectListEventArgs> OnPlayerAgentsInstantiated;

    void Start()
    {
        List<GameObject> instances = new List<GameObject>();

        foreach(var r in Singleton<PlayerDataManager>.Instance.ActivatedPlayerSessions)
        {
            GameObject g = InitGameObjectByDataSession(r);

            if(g != null) instances.Add(g);
        }

        OnPlayerAgentsInstantiated?.Invoke(this, new CSEventArgs.GameObjectListEventArgs(instances));
    }

    void OnEnable()
    {
        PlayerDataManager.OnPlayerActivated += PlayerDataManager_OnPlayerActivated;
    }
    void OnDisable()
    {
        PlayerDataManager.OnPlayerActivated -= PlayerDataManager_OnPlayerActivated;
    }
    private void PlayerDataManager_OnPlayerActivated(object sender, PlayerDataManager.PlayerDataSessionEventArgs e)
    {
        var g = InitGameObjectByDataSession(e.Data);

        List<GameObject> Glist = new List<GameObject>();
        Glist.Add(g);

        OnPlayerAgentsInstantiated?.Invoke(this, new CSEventArgs.GameObjectListEventArgs(Glist));
    }

    GameObject InitGameObjectByDataSession(PlayerData_Session r)
    {
        GameObject g;

        if (ForceUseSpawnQueue || r.Info._CurrentClassPreset == null)
        {
            g = ClassSpawnQueue[_CurrentClassSpawnIndex]?.InstantiatePlayerActor();
            _CurrentClassSpawnIndex += 1 % ClassSpawnQueue.Count;

            if (g != null)
            {
                InitPlayerActor(g, r.Info._Input);
            }
        }
        else
        {
            g = r.Info._CurrentClassPreset?.InstantiatePlayerActor();

            if (g != null)
            {
                InitPlayerActor(g, r.Info._Input);
            }
        }

        return g;
    }
    void InitPlayerActor(GameObject g, PlayerInput p)
    {
        g.transform.position = transform.position;

        var h = g.GetComponent<PlayerInputHost>();
        h.CurrentPlayerInput = p;
    }
}
