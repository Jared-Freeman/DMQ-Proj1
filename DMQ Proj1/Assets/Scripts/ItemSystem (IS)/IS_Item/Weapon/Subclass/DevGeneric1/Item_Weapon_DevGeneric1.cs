﻿using System.Collections;
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
        public bool UseNoCollideLayer = true;

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
                        var instance = GenericProjectile.SpawnProjectile(p, ctx._InitialPosition, iDir, new Vector2(iDir.x, iDir.z), ctx._TargetGameObject, ctx._Owner);

                        if (ctx._Owner != null)
                        {
                            if (UseNoCollideLayer)
                            {
                                instance.gameObject.layer = ctx._Owner._Team.Options.NoCollideLayer;
                                foreach (Transform child in instance.transform)  //make sure to modify all objects in hierarchy!
                                {
                                    child.gameObject.layer = ctx._Owner._Team.Options.NoCollideLayer;
                                }
                            }
                            else
                            {
                                instance.gameObject.layer = ctx._Owner._Team.Options.Layer;
                                foreach (Transform child in instance.transform)
                                {
                                    child.gameObject.layer = ctx._Owner._Team.Options.Layer;
                                }
                            }
                        }


                        return true;
                    }
                    else
                    {
                        if (s_FLAG_ITEM_DEBUG) Debug.Log("projectile component not found");
                    }
                }
                else
                {
                    if (s_FLAG_ITEM_DEBUG) Debug.Log("Cooldown unavailable.");
                }
            }
            return false;
        }


    }
}