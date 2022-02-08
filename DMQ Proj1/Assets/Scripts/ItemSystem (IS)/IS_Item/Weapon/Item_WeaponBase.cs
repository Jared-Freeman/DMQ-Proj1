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

        public override IS_ItemPresetBase Preset
        {
            get
            {
                var p = base.Preset as IS_WeaponPreset;
                if (p != null)
                {
                    return p;
                }
                return null;
            }
            set
            {
                var p = value as IS_WeaponPreset;
                if (p != null)
                {
                    base.Preset = p;
                }
            }
        }

        public bool CanAttack { get { return BaseWeaponInfo._Cooldown.CanUseCooldown(); } }

        new protected virtual void Awake()
        {
            base.Awake();

            if (Preset == null)
            {
                Debug.LogError(ToString() + ": Invalid Preset attached! Destroying");
                Destroy(gameObject);
            }

            //Instantiate a Cooldown Tracker via copy ctor for this weapon instance
            BaseWeaponInfo._Cooldown = new Utils.CooldownTracker((Preset as IS_WeaponPreset)?.BaseWeaponOptions.CooldownPreset);
        }

        //This impl is the fairly standard way of overriding the base method.
        //You can honestly just let the base version do its thing most of the time
        /// <summary>
        /// Disables the gameObject and returns true
        /// </summary>
        /// <returns></returns>
        new public virtual bool AddItemToInventorySpace(IS_InventoryBase inv)
        {
            //base is sufficient for this class
            base.AddItemToInventorySpace(inv);

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
