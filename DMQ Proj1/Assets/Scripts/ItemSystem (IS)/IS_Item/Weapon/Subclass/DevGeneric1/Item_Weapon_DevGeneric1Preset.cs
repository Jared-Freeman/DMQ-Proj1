using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItemSystem;

namespace ItemSystem.Weapons
{
    /// <summary>
    /// Preset for generic weapon 1. All extra options (NOT state info) for this weapon subclass is stored here.
    /// </summary>
    [CreateAssetMenu(fileName = "WEP_", menuName = "ScriptableObjects/Items/Weapon Presets/Dev Generic 1", order = 2)]
    public class Item_Weapon_DevGeneric1Preset : IS_WeaponPreset
    {
        public GameObject ProjectilePrefab;
    }
}