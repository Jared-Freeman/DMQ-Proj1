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
    public CharacterClass DefaultCharacterClassPreset;

    /// <summary>
    /// Fires when the gameobjects that represent the players are created and initialized
    /// </summary>
    public static event System.EventHandler<CSEventArgs.GameObjectListEventArgs> OnPlayerAgentsInstantiated;

    void Start()
    {
        List<GameObject> instances = new List<GameObject>();

        foreach(var r in Singleton<PlayerDataManager>.Instance.ActivatedPlayerSessions)
        {
            GameObject g;

            if(r.Info._CurrentClassPreset == null)
            {
                g = DefaultCharacterClassPreset?.InstantiatePlayerActor();

                if (g != null)
                {
                    InitPlayerActor(g, r.Info._Input);
                }
            }
            else
            {
                g = r.Info._CurrentClassPreset?.InstantiatePlayerActor();

                if(g != null)
                {
                    InitPlayerActor(g, r.Info._Input);
                }
            }

            if(g != null) instances.Add(g);
        }

        OnPlayerAgentsInstantiated?.Invoke(this, new CSEventArgs.GameObjectListEventArgs(instances));
    }

    void InitPlayerActor(GameObject g, PlayerInput p)
    {
        g.transform.position = transform.position;

        var h = g.GetComponent<PlayerInputHost>();
        h.CurrentPlayerInput = p;
    }
}
