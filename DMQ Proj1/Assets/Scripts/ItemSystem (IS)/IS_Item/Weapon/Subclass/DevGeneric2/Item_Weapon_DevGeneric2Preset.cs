using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ItemSystem.Weapons
{
    [CreateAssetMenu(fileName = "WEP_", menuName = "ScriptableObjects/Items/Weapon Presets/Dev Generic 2", order = 1)]
    public class Item_Weapon_DevGeneric2Preset : IS_WeaponPreset
    {
        public AbilitySystem.AS_Ability_Base Ability;
    }
}