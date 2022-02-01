﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ItemSystem.Weapons
{

    /// <summary>
    /// Generic Weapon Item. Should be extended to add an implementation to attack method(s)
    /// </summary>
    public class Item_WeaponBase : ItemSystem.IS_ItemBase
    {
        [System.Serializable]
        public struct AddedInfo
        {
            public Utils.CooldownTracker _Cooldown; //may have multiple cooldowns
        }

        protected AddedInfo BaseWeaponInfo;
        public ItemSystem.IS_WeaponPreset BaseWeaponData;

        public bool CanAttack { get { return BaseWeaponInfo._Cooldown.CanUseCooldown(); } }

        new protected virtual void Awake()
        {
            base.Awake();

            if (BaseWeaponData == null)
            {
                Debug.LogError(ToString() + ": No Data attached! Destroying");
                Destroy(gameObject);
            }

            //Instantiate a Cooldown Tracker via copy ctor for this weapon instance
            BaseWeaponInfo._Cooldown = new Utils.CooldownTracker(BaseWeaponData.BaseWeaponOptions.CooldownPreset);
        }

        //This impl is the fairly standard way of overriding the base method.
        //You can honestly just let the base version do its thing most of the time
        /// <summary>
        /// Disables the gameObject and returns true
        /// </summary>
        /// <returns></returns>
        new public virtual bool AddItemToInventorySpace()
        {
            //base is sufficient for this class
            base.AddItemToInventorySpace();

            return true;
        }


        /// <summary>
        /// Empty Attack. Checks that AttackContext != null
        /// </summary>
        /// <param name="ctx">Supplied AttackContext</param>
        /// <returns>True if the supplied AttackContext exists</returns>
        public virtual bool InvokeAttack(AttackContext ctx)
        {
            return (ctx != null);
        }

        /// <summary>
        /// Container for any and all context needed to generate any attack. 
        /// Not every member of this struct will be utilized for every attack.
        /// Designers are free to interpret this context as they please. 
        /// Context is generated by some combination of IO and state variables (on, for instance, a Player Actor)
        /// NOTE: Members are NOT guaranteed to be non-null!
        /// </summary>
        public class AttackContext
        {
            public Actor _Owner = null;
            public Team _Team = null;

            public GameObject _InitialGameObject = null;
            public Vector3 _InitialPosition = Vector3.zero;
            public Vector3 _InitialDirection = Vector3.zero;

            public GameObject _TargetGameObject = null;
            public Vector3 _TargetPosition = Vector3.zero;
            public Vector3 _TargetDirection = Vector3.zero;
        }
    }
}
