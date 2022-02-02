using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem.Weapons.HeavyFlintlock
{
    public class AS_HeavyFlintlock_Bulwark_Atk1 : AS_Ability_Instance_Base
    {
        [SerializeField] protected AS_HeavyFlintlock_Bulwark_Atk1_Preset SubclassSettings;            

        public override AS_Ability_Base Settings 
        { 
            get => SubclassSettings; 
            set
            {
                SubclassSettings = value as AS_HeavyFlintlock_Bulwark_Atk1_Preset;

                if (SubclassSettings == null)
                {
                    Debug.LogError("Ability settings are wrong for this Ability. Was the correct sub");
                }
            }
        }

        public override void Awake()
        {
            base.Awake();
        }

        public override bool ExecuteAbility()
        {
            Debug.Log(ToString());
            return base.ExecuteAbility();
        }
    }

    public class AS_HeavyFlintlock_Bulwark_Atk1_Preset : AS_Ability_Base
    {
        public override AS_Ability_Instance_Base GetInstance(GameObject ability_owner)
        {
            base.GetInstance(ability_owner);            

            return ability_owner.AddComponent<AS_HeavyFlintlock_Bulwark_Atk1>();
        }
    }
}