using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ClassSystem;

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
                    g.transform.position = transform.position;
            }
            else
            {
                g = r.Info._CurrentClassPreset?.InstantiatePlayerActor();

                if(g != null)
                    g.transform.transform.position = transform.position;    
            }
        }
    }
}
