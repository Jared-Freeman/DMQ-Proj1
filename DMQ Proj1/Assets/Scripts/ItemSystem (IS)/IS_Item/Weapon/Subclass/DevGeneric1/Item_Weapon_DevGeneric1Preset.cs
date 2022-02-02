using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItemSystem;


//We now use a convention to offer multiple variations per class. I feel this may be the best re-tooling of our current system for that.
namespace ItemSystem.Weapons
{
    /// <summary>
    /// Preset for generic weapon 1. All extra options (NOT state info) for this weapon subclass is stored here.
    /// </summary>
    [CreateAssetMenu(fileName = "WEP_", menuName = "ScriptableObjects/Items/Weapon Presets/Dev Generic 1/Default", order = 2)]
    public class Item_Weapon_DevGeneric1Preset : IS_WeaponPreset
    {
        public AbilitySystem.AS_Ability_Base Ability_BasicAttack;
    }

    [CreateAssetMenu(fileName = "WEP_", menuName = "ScriptableObjects/Items/Weapon Presets/Dev Generic 1/Interdictor", order = 2)]
    public class Item_Weapon_DevGeneric1_Interdictor : IS_WeaponPreset
    {
        public GameObject ProjectilePrefab;
    }

    [CreateAssetMenu(fileName = "WEP_", menuName = "ScriptableObjects/Items/Weapon Presets/Dev Generic 1/Bulwark", order = 2)]
    public class Item_Weapon_DevGeneric1_Bulwark : IS_WeaponPreset
    {
        public GameObject ProjectilePrefab;
    }

    [CreateAssetMenu(fileName = "WEP_", menuName = "ScriptableObjects/Items/Weapon Presets/Dev Generic 1/Arcanist", order = 2)]
    public class Item_Weapon_DevGeneric1_Arcanist : IS_WeaponPreset
    {
        public GameObject ProjectilePrefab;
    }

    [CreateAssetMenu(fileName = "WEP_", menuName = "ScriptableObjects/Items/Weapon Presets/Dev Generic 1/Enchanter", order = 2)]
    public class Item_Weapon_DevGeneric1_Enchanter : IS_WeaponPreset
    {
        public GameObject ProjectilePrefab;
    }
}