using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ItemSystem
{
    [CreateAssetMenu(fileName = "WEP_", menuName = "ScriptableObjects/Items/Weapon Preset", order = 1)]
    public class IS_WeaponPreset : ItemSystem.IS_ItemPresetBase
    {
        [System.Serializable]
        public struct IS_WeaponOptions
        {
            public float EquipTime;
            public float DequipTime;

            public Utils.CooldownTracker CooldownPreset; //This is a preset. Dont call this cooldown directly -- just reference its options

            //new ability stuff 2-27-2022
            public AbilitySystem.AS_Ability_Base Ability_Attack;
            public AbilitySystem.AS_Ability_Base Ability_Ability1;
            public AbilitySystem.AS_Ability_Base Ability_Ability2;

            public int AnimatorIndex; //Used to tell the animator which weapon is equipped.
        }

        public IS_WeaponOptions BaseWeaponOptions;


    }
}