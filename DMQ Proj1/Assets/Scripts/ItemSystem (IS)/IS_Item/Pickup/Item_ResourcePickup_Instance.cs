using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using EffectTree;

namespace ItemSystem
{
    /// <summary>
    /// This type of item dispatches to an Effect Tree when Destroy'd.
    /// Common uses would be restoring the Player's energy, HP, or charges of consumables
    /// </summary>
    public class Item_ResourcePickup_Instance : IS_ItemBase
    {
        #region Members

        public override IS_ItemPresetBase Preset
        {
            get
            {
                var p = base.Preset as Item_ResourcePickup_Preset;
                if (p != null)
                {
                    return p;
                }
                return null;
            }
            set
            {
                var p = value as Item_ResourcePickup_Preset;
                if (p != null)
                {
                    base.Preset = p;
                }
            }
        }
        /// <summary>
        /// Convenience property that casts base Preset
        /// </summary>
        public Item_ResourcePickup_Preset RP_Preset { get { return Preset as Item_ResourcePickup_Preset; } set { Preset = value; } }

        #endregion

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        protected override void OnItemAddedToInventory(IS_InventoryBase inv)
        {
            base.OnItemAddedToInventory(inv);

            Utils.AttackContext c = new Utils.AttackContext();

            c._InitialDirection = (inv.gameObject.transform.position - transform.position);
            c._InitialGameObject = gameObject;
            c._InitialPosition = transform.position;

            c._Owner = inv.GetComponent<Actor>();
            c._Team = inv.GetComponent<Actor>()?._Team;

            c._TargetGameObject = inv.gameObject;
            c._TargetPosition = inv.gameObject.transform.position;
            c._TargetDirection = (inv.gameObject.transform.position - transform.position);

            EffectContext ec = new EffectContext(c);

            RP_Preset.PickupEffect.Invoke(ref ec);
        }
    }

}