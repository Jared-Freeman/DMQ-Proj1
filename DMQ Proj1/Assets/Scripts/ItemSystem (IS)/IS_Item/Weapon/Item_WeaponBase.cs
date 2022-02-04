using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

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

    }

}
