using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ItemSystem.Weapons
{
    /// <summary>
    /// Generic weapon for testing purposes. 
    /// </summary>
    public class Item_Weapon_DevGeneric1 : Item_WeaponBase
    {
        public Item_Weapon_DevGeneric1Preset Preset;

        protected override void Awake()
        {
            base.Awake();

            if(Preset == null)
            {
                Debug.LogError(ToString() + ": No preset SO specified! Destroying this");
                Destroy(this);
            }
            else if(Preset.ProjectilePrefab == null)
            {
                Debug.LogError(ToString() + ": No preset Projectile Prefab specified! Destroying this");
                Destroy(this);
            }
        }


        /// <summary>
        /// Generic attack.
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns>True if attack succeeds</returns>
        public override bool InvokeAttack(AttackContext ctx)
        {
            if( base.InvokeAttack(ctx))
            {
                if(BaseWeaponInfo._Cooldown.CooldownAvailable)
                {
                    BaseWeaponInfo._Cooldown.ConsumeCooldown();

                    var p = Preset.ProjectilePrefab.GetComponent<GenericProjectile>();
                    if(p != null)
                    {
                        var iDir = ctx._InitialDirection;
                        GenericProjectile.SpawnProjectile(p, ctx._InitialPosition, iDir, new Vector2(iDir.x, iDir.z), ctx._TargetGameObject, ctx._Owner);


                        return true;
                    }
                }
            }
            return false;
        }


    }
}