using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

using AbilitySystem;

namespace ItemSystem.Weapons
{

    /// <summary>
    /// Instance for DevGeneric2
    /// </summary>
    public class Item_Weapon_DevGeneric2 : Item_WeaponBase
    {
        public Item_Weapon_DevGeneric2Preset DevGeneric2Preset;

        protected StateInfo DevGeneric2_Info;

        [System.Serializable]
        protected struct StateInfo
        {
            public AS_Ability_Instance_Base AttachedAbilityInstance;
        }

        protected override void Awake()
        {
            base.Awake();

            if (DevGeneric2Preset == null)
            {
                Debug.LogError("Preset not set!!");
                Destroy(gameObject);
            }

            DevGeneric2_Info.AttachedAbilityInstance = DevGeneric2Preset.Ability.GetInstance(gameObject);

        }

        public override bool InvokeAttack(AttackContext ctx)
        {
            if(base.InvokeAttack(ctx))
            {
                EffectTree.EffectContext c = new EffectTree.EffectContext();

                c.AttackData = ctx;
                
                DevGeneric2_Info.AttachedAbilityInstance.ExecuteAbility(ref c);

                return true;
            }
            return false;
        }
    }
}