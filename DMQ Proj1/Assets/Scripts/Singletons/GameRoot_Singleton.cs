using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An empty class that allows designers to mark a GameObject as an accessible Singleton root for other singleton members.
/// Useful for establishing all Singletons into a single hierarchy prefab asset
/// </summary>
public class GameRoot_Singleton : Singleton<GameRoot_Singleton>
{
    protected override void Awake()
    {
        if (Singleton<GameRoot_Singleton>.InstanceExists)
        {
            Destroy(gameObject);
        }
        else
        {
            base.Awake();
        }
    }
}
