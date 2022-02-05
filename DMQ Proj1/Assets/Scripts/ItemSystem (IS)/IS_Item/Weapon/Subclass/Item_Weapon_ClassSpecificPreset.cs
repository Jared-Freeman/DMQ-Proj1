using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AbilitySystem;
using ClassSystem;

namespace ItemSystem.Weapons
{
    [CreateAssetMenu(fileName = "WEP_", menuName = "ScriptableObjects/Items/Weapon Presets/Class Specific Weapon", order = 1)]
    public class Item_Weapon_ClassSpecificPreset : IS_WeaponPreset
    {

        public List<ClassAbilityRecord> ListAbilitiesPerClass = new List<ClassAbilityRecord>();

        public AS_Ability_Base DefaultBasicAttack;
        public AS_Ability_Base DefaultAbility_1;
        public AS_Ability_Base DefaultAbility_2;

        /// <summary>
        /// A record of abilities, per class
        /// </summary>
        [System.Serializable]
        public class ClassAbilityRecord
        {
            public CharacterClass Class;
            public AS_Ability_Base BasicAttack;
            public AS_Ability_Base Ability_1;
            public AS_Ability_Base Ability_2;
        }

        /// <summary>
        /// Gets record for this class, if it exists
        /// </summary>
        /// <param name="c"></param>
        /// <returns>record for this class or null if it doesnt exist</returns>
        public ClassAbilityRecord GetRecord(ClassSystem.CharacterClass c)
        {
            foreach(var r in ListAbilitiesPerClass)
            {
                if (c == r.Class) return r;
            }
            return null;
        }
    }

}