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
        public Item_Weapon_DevGeneric1Preset DefaultWeaponPreset;
        public bool UseNoCollideLayer = true;
        
        // To extend functionality for multiple classes
        // If the user has one of the classes with different functionality it will play that attack. Else it will play the default attack
        public Item_Weapon_DevGeneric1_Arcanist _W_Arcanist;
        public Item_Weapon_DevGeneric1_Bulwark _W_Bulwark;
        public Item_Weapon_DevGeneric1_Enchanter _W_Enchanter;
        public Item_Weapon_DevGeneric1_Interdictor _W_Interdictor;

        protected override void Awake()
        {
            base.Awake();

            if(DefaultWeaponPreset == null)
            {
                Debug.LogError(ToString() + ": No DefaultWeaponPreset SO specified! Destroying this");
                Destroy(this);
            }
            else if(DefaultWeaponPreset.ProjectilePrefab == null)
            {
                Debug.LogError(ToString() + ": No DefaultWeaponPreset Projectile Prefab specified! Destroying this");
                Destroy(this);
            }
        }


        /// <summary>
        /// Generic attack. Variations depending on class.
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns>True if attack succeeds</returns>
        public override bool InvokeAttack(AttackContext ctx)
        {
            if (s_FLAG_ITEM_DEBUG) Debug.Log("Invoking ctx");


            if( base.InvokeAttack(ctx))
            {
                //Class branches
                Actor_Player A = ctx._Owner as Actor_Player;

                if(A == null)
                {
                    if (s_FLAG_ITEM_DEBUG) Debug.Log("no player invoke!");
                    return DefaultAttack(ctx);
                }
                else
                {
                    //Simple string matching to class SO name to change weapon attack functionality.
                    ClassSystem.CharacterClass invokingClass;
                    invokingClass = A.Class;

                    //probably dont want to hardcode this w these string literals...
                    if (invokingClass.ClassName.ToLower() == "interdictor")
                    {
                        if (s_FLAG_ITEM_DEBUG) Debug.Log("interdictor invoke!");
                    }
                    else if (invokingClass.ClassName.ToLower() == "bulwark")
                    {
                        if (s_FLAG_ITEM_DEBUG) Debug.Log("bulwark invoke!");
                    }
                    else if (invokingClass.ClassName.ToLower() == "arcanist")
                    {
                        if(s_FLAG_ITEM_DEBUG) Debug.Log("Arcanist invoke!");
                        ArcanistAttack(ctx);
                    }
                    else if (invokingClass.ClassName.ToLower() == "enchanter")
                    {
                        if (s_FLAG_ITEM_DEBUG) Debug.Log("enchanter invoke!");
                    }
                    else
                    {
                        if (s_FLAG_ITEM_DEBUG) Debug.Log("default invoke!");
                        return DefaultAttack(ctx);
                    }
                }
                
            }
            return false;
        }

        //helpers for attack invocation

        public bool DefaultAttack(AttackContext ctx)
        {

            GenericProjectile instance = Utils_IS_Weapon.CreateProjectileFromAttackContext(
                DefaultWeaponPreset.ProjectilePrefab.GetComponent<GenericProjectile>()
                , ctx);

            if (instance == null) return false;

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


        public bool InterdictorAttack(AttackContext ctx)
        {
            return false;
        }
        public bool BulwarkAttack(AttackContext ctx)
        {
            return false;
        }
        public bool EnchanterAttack(AttackContext ctx)
        {
            return false;
        }
        //TODO: Cooldowns PER class weapon
        public bool ArcanistAttack(AttackContext ctx)
        {
            GenericProjectile instance = Utils_IS_Weapon.CreateProjectileFromAttackContext(
                _W_Arcanist.ProjectilePrefab.GetComponent<GenericProjectile>()
                , ctx);

            if (instance == null) return false;

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


            return false;
        }
    }
}