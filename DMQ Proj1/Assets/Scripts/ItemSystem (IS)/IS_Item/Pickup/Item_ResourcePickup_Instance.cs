using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ItemSystem
{
    /// <summary>
    /// This type of item dispatches to an Effect Tree when picked up. 
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


    }

}