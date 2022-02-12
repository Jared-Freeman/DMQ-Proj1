using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ClassSystem;
using UnityEngine.InputSystem;

/// <summary>
/// Looks at ActivatedPlayers and spawns the appropriate player agent in
/// </summary>
public class DEBUG_PlayerSpawner : MonoBehaviour
{
    public CharacterClass DefaultCharacterClassPreset;

    void Start()
    {
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
        }
    }

    void InitPlayerActor(GameObject g, PlayerInput p)
    {
        g.transform.position = transform.position;

        var h = g.GetComponent<PlayerInputHost>();
        h.CurrentPlayerInput = p;
    }
}
