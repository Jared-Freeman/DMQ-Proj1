using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Instantiated Weapon Action. Is persistent until destroyed by the designer, allowing for more complex actions than just physics overlap or something.
/// </summary>
public class WeaponActionBase : MonoBehaviour
{
    public WeaponActionPreset BasePreset;
    public BaseWepInfo BaseInfo { get; protected set; }

    [System.Serializable]
    public struct BaseWepInfo
    {
        public Team _Team;
        public Actor _Creator;
    }

    public virtual void PerformAction() { }
}

/// <summary>
/// Holds specific settings per-weapon. A WeaponAction should be instantiated in some form
/// </summary>
public class WeaponActionPreset : ScriptableObject
{
    public BaseWepOptions BaseOptions;

    [System.Serializable]
    public struct BaseWepOptions
    {

    }
}
