using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

using AbilitySystem;

namespace ItemSystem.Weapons
{
    public class Item_Weapon_DevBoomerang : Item_WeaponBase
    {
        public Item_Weapon_DevBoomerangPreset DevBoomerangPreset;

        protected StateInfo DevBoomerang_Info;

        [System.Serializable]
        protected struct StateInfo
        {
            public AS_Ability_Instance_Base AttachedAbilityInstance;
        }

        protected override void Awake()
        {
            base.Awake();

            if(DevBoomerangPreset == null)
            {
                Debug.LogError("Preset not set!!");
                Destroy(gameObject);
            }

            DevBoomerang_Info.AttachedAbilityInstance = DevBoomerangPreset.Ability.GetInstance(gameObject);
        }

        public override bool InvokeAttack(AttackContext ctx)
        {
            if(base.InvokeAttack(ctx))
            {
                EffectTree.EffectContext c = new EffectTree.EffectContext();
                c.AttackData = ctx;

                if(DevBoomerang_Info.AttachedAbilityInstance)
                {
                    DevBoomerang_Info.AttachedAbilityInstance.ExecuteAbility(ref c);
                    return true;
                }
            }
            return false;
        }
    }
}

