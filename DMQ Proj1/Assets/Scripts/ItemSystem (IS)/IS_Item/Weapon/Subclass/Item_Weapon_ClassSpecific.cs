using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ClassSystem;
using AbilitySystem;
using Utils;
using EffectTree;


namespace ItemSystem.Weapons
{
    public class Item_Weapon_ClassSpecific_EventArgs : System.EventArgs
    {
        public Item_Weapon_ClassSpecific _Weapon;

        public Item_Weapon_ClassSpecific_EventArgs(Item_Weapon_ClassSpecific weapon)
        {
            _Weapon = weapon;
        }
    }

}

namespace ItemSystem.Weapons
{
    public class Item_Weapon_ClassSpecific : Item_WeaponBase
    {
        #region Events

        public static event System.EventHandler<Item_Weapon_ClassSpecific_EventArgs> OnAbility1Invoked;
        public static event System.EventHandler<Item_Weapon_ClassSpecific_EventArgs> OnAbility2Invoked;
        public static event System.EventHandler<Item_Weapon_ClassSpecific_EventArgs> OnAbilityBasicAttackInvoked;

        #endregion

        #region Members

        protected ItemWeapClassSpecific_StateInfo IWCS_StateInfo;

        #region Properties

        public override IS_ItemPresetBase Preset 
        { 
            get
            {
                var p = base.Preset as Item_Weapon_ClassSpecificPreset;
                if (p != null)
                {
                    return p;
                }
                return null;
            }
            set
            {
                var p = value as Item_Weapon_ClassSpecificPreset;
                if(p != null)
                {
                    base.Preset = p;
                }               
            } 
        }

        public override bool CanAttack
        {
            get => IWCS_StateInfo.CurrentBasicAttack.CanCastAbility;
        }
        public override bool CanAbility1
        {
            get => IWCS_StateInfo.CurrentAbility_1.CanCastAbility;
        }
        public override bool CanAbility2
        {
            get => IWCS_StateInfo.CurrentAbility_2.CanCastAbility;
        }
        /// <summary>
        /// Class of player that currently owns this weapon instance. Can be null
        /// </summary>
        public ClassSystem.CharacterClass CurrentClass
        {
            get => IWCS_StateInfo.CurrentClass;
        }
        /// <summary>
        /// Player that currently owns this weapon instance. Can be null
        /// </summary>
        public Actor_Player CurrentPlayer
        {
            get => IWCS_StateInfo.CurrentPlayer;
        }

        //for internal convenience
        private Item_Weapon_ClassSpecificPreset IWCS_Preset
        {
            get
            {
                return Preset as Item_Weapon_ClassSpecificPreset;
            }
        }

        #endregion

        #region Helpers

        protected struct ItemWeapClassSpecific_StateInfo
        {
            public ClassSystem.CharacterClass CurrentClass;
            public AS_Ability_Instance_Base CurrentBasicAttack;
            public AS_Ability_Instance_Base CurrentAbility_1;
            public AS_Ability_Instance_Base CurrentAbility_2;

            public Actor_Player CurrentPlayer;

            /// <summary>
            /// Cleans up and removes state info refs
            /// </summary>
            public void ClearStateInfo()
            {
                if(CurrentBasicAttack != null) Destroy(CurrentBasicAttack);
                if(CurrentAbility_2 != null) Destroy(CurrentAbility_2);
                if (CurrentAbility_1 != null) Destroy(CurrentAbility_1);

                CurrentClass = null;
                CurrentBasicAttack = null;
                CurrentAbility_1 = null;
                CurrentAbility_2 = null;
                CurrentPlayer = null;
            }
        }

        #endregion

        #endregion

        #region Initialization

        protected override void Awake()
        {
            base.Awake();

            if(IWCS_Preset == null)
            {
                Debug.LogError(ToString() + " preset is null! Destroying.");
                Destroy(this);
            }
        }

        protected virtual void Start()
        {
            IS_InventoryBase.Event_ItemEntersInventory += IS_InventoryBase_Event_ItemEntersInventory;
            IS_InventoryBase.Event_ItemLeavesInventory += IS_InventoryBase_Event_ItemLeavesInventory; ;
        }


        protected override void OnDestroy()
        {
            base.OnDestroy();
            IS_InventoryBase.Event_ItemEntersInventory -= IS_InventoryBase_Event_ItemEntersInventory;
            IS_InventoryBase.Event_ItemLeavesInventory -= IS_InventoryBase_Event_ItemLeavesInventory;
        }

        #endregion

        #region Event Handlers


        /// <summary>
        /// Checks if this item entered a player inventory, and generates the appropriate ability for its state info
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IS_InventoryBase_Event_ItemEntersInventory(object sender, CSEventArgs.ItemAndInventoryEventArgs e)
        {
            if(e.Item == this && e.Inventory as Inventory_WeaponSlot != null)
            {
                if (s_FLAG_ITEM_DEBUG) Debug.Log("Weaponslot equip detected. Adding class-specific ability instances, if they exist!");

                var a = e.Inventory.GetComponent<Actor_Player>();

                if(a != null) 
                {
                    IWCS_StateInfo.CurrentPlayer = a;
                    IWCS_StateInfo.CurrentClass = a.Class;

                    Item_Weapon_ClassSpecificPreset.ClassAbilityRecord r = IWCS_Preset.GetRecord(a.Class);
                    if (r != null)
                    {
                        //custom init of abils based on class record
                        IWCS_StateInfo.CurrentAbility_1 = r.Ability_1.GetInstance(a.gameObject);
                        IWCS_StateInfo.CurrentAbility_2 = r.Ability_2.GetInstance(a.gameObject);
                        IWCS_StateInfo.CurrentBasicAttack = r.BasicAttack.GetInstance(a.gameObject);

                        return; // uh just notice that this is here...
                    }
                }

                //default init of abils
                IWCS_StateInfo.CurrentAbility_1 = IWCS_Preset.DefaultAbility_1.GetInstance(e.Inventory.gameObject);
                IWCS_StateInfo.CurrentAbility_2 = IWCS_Preset.DefaultAbility_2.GetInstance(e.Inventory.gameObject);
                IWCS_StateInfo.CurrentBasicAttack = IWCS_Preset.DefaultBasicAttack.GetInstance(e.Inventory.gameObject);
            }
        }

        /// <summary>
        /// Checks if this item left an inventory, and destroys the appropriate refs in its state info
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IS_InventoryBase_Event_ItemLeavesInventory(object sender, CSEventArgs.ItemAndInventoryEventArgs e)
        {
            if(e.Item == this)
            {
                IWCS_StateInfo.ClearStateInfo();
            }
        }

        #endregion

        public override bool InvokeAttack(AttackContext ctx)
        {
            if( base.InvokeAttack(ctx))
            {
                if (IWCS_StateInfo.CurrentBasicAttack == null) return false;

                EffectContext ec = new EffectContext(ctx);

                OnAbilityBasicAttackInvoked?.Invoke(this, new Item_Weapon_ClassSpecific_EventArgs(this));

                return IWCS_StateInfo.CurrentBasicAttack.ExecuteAbility(ref ec);
            }
            return false;
        }

        public override bool InvokeAbility1(AttackContext ctx)
        {
            if( base.InvokeAbility1(ctx))
            {
                if (IWCS_StateInfo.CurrentAbility_1 == null) return false;

                EffectTree.EffectContext ec = new EffectTree.EffectContext(ctx);

                OnAbility1Invoked?.Invoke(this, new Item_Weapon_ClassSpecific_EventArgs(this));

                return (IWCS_StateInfo.CurrentAbility_1.ExecuteAbility(ref ec));
            }
            return false;
        }

        public override bool InvokeAbility2(AttackContext ctx)
        {
            if( base.InvokeAbility2(ctx))
            {
                if (IWCS_StateInfo.CurrentAbility_2 == null) return false;

                EffectTree.EffectContext ec = new EffectTree.EffectContext(ctx);

                OnAbility2Invoked?.Invoke(this, new Item_Weapon_ClassSpecific_EventArgs(this));

                return (IWCS_StateInfo.CurrentAbility_2.ExecuteAbility(ref ec));
            }
            return false;
        }
    }
}