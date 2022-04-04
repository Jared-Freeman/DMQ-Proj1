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
        #region Members

        protected AddedInfo BaseWeaponInfo;

        #region Properties

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
        /// <summary>
        /// Convenience property. Returns base Preset as an IS_WeaponPreset
        /// </summary>
        public IS_WeaponPreset BaseWeaponPreset
        {
            get => Preset as IS_WeaponPreset;
            set { Preset = value; }
        }

        //its a good idea to override these if you plan on not using the base Ability impl for invoking attacks
        public virtual bool CanAttack { get { return BaseWeaponInfo.AbilInstance_Attack.CanCastAbility; } }
        public virtual bool CanAbility1 { get { return BaseWeaponInfo.AbilInstance_Ability1.CanCastAbility; } }
        public virtual bool CanAbility2 { get { return BaseWeaponInfo.AbilInstance_Ability2.CanCastAbility; } }

        #endregion

        #region Helpers

        [System.Serializable]
        public struct AddedInfo
        {
            public Utils.CooldownTracker _Cooldown; //may have multiple cooldowns

            public AbilitySystem.AS_Ability_Instance_Base AbilInstance_Attack;
            public AbilitySystem.AS_Ability_Instance_Base AbilInstance_Ability1;
            public AbilitySystem.AS_Ability_Instance_Base AbilInstance_Ability2;
        }
        #endregion

        #endregion

        new protected virtual void Awake()
        {
            base.Awake();

            if (Preset == null)
            {
                Debug.LogError(ToString() + ": Invalid Preset attached! Destroying");
                Destroy(gameObject);
            }

            //Instantiate a Cooldown Tracker via copy ctor for this weapon instance
            BaseWeaponInfo._Cooldown = new Utils.CooldownTracker(BaseWeaponPreset.BaseWeaponOptions.CooldownPreset); //kinda obsolete...

            if(BaseWeaponPreset.BaseWeaponOptions.Ability_Attack)
                BaseWeaponInfo.AbilInstance_Attack = BaseWeaponPreset.BaseWeaponOptions.Ability_Attack.GetInstance(gameObject);
            if(BaseWeaponPreset.BaseWeaponOptions.Ability_Ability1)
                BaseWeaponInfo.AbilInstance_Ability1 = BaseWeaponPreset.BaseWeaponOptions.Ability_Ability1.GetInstance(gameObject);
            if(BaseWeaponPreset.BaseWeaponOptions.Ability_Ability2)
                BaseWeaponInfo.AbilInstance_Ability2 = BaseWeaponPreset.BaseWeaponOptions.Ability_Ability2.GetInstance(gameObject);
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
        /// Tries to cast ability
        /// </summary>
        /// <param name="ctx">Supplied AttackContext</param>
        /// <returns>True if the Ability was cast.</returns>
        /// <remarks>
        /// As of now, this base method does NOT invoke an ability -- 
        /// instead leaving that responsibility to subclasses. However, the implementation has been written.
        /// </remarks>
        public virtual bool InvokeAttack(AttackContext ctx)
        {
            if (ctx == null) return false;
            else return true;

            EffectTree.EffectContext ec = new EffectTree.EffectContext(ctx);

            return (BaseWeaponInfo.AbilInstance_Attack.ExecuteAbility(ref ec));
        }
        /// <summary>
        /// Tries to cast ability
        /// </summary>
        /// <param name="ctx">Supplied AttackContext</param>
        /// <returns>True if the Ability was cast.</returns>
        /// <remarks>
        /// As of now, this base method does NOT invoke an ability -- 
        /// instead leaving that responsibility to subclasses. However, the implementation has been written.
        /// </remarks>
        public virtual bool InvokeAbility1(AttackContext ctx)
        {
            if (ctx == null) return false;
            else return true;

            EffectTree.EffectContext ec = new EffectTree.EffectContext(ctx);

            return (BaseWeaponInfo.AbilInstance_Ability1.ExecuteAbility(ref ec));
        }
        /// <summary>
        /// Tries to cast ability
        /// </summary>
        /// <param name="ctx">Supplied AttackContext</param>
        /// <returns>True if the Ability was cast.</returns>
        /// <remarks>
        /// As of now, this base method does NOT invoke an ability -- 
        /// instead leaving that responsibility to subclasses. However, the implementation has been written.
        /// </remarks>
        public virtual bool InvokeAbility2(AttackContext ctx)
        {
            if (ctx == null) return false;
            else return true;

            EffectTree.EffectContext ec = new EffectTree.EffectContext(ctx);

            return (BaseWeaponInfo.AbilInstance_Ability2.ExecuteAbility(ref ec));
        }

    }

}
