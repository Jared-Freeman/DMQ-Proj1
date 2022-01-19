﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSEventArgs
{
    public class ItemEventArgs : System.EventArgs
    {
        public ItemSystem.IS_ItemBase Item;
        public ItemEventArgs(ItemSystem.IS_ItemBase item)
        {
            Item = item;
        }
    }
}

namespace ItemSystem
{
    public enum ItemLocation { Inventory, World } //To distinguish between items in inventories or elsewhere


    /// <summary>
    /// Base class for an instantiated item. Extending this should be paired with an IS_ItemPresetBase subclass impl!
    /// </summary>
    public abstract class IS_ItemBase : MonoBehaviour
    {
        #region Events

        public static event System.EventHandler<CSEventArgs.ItemEventArgs> OnItemDestroyed;

        #endregion

        #region Members
        //preset base
        public IS_ItemPresetBase BasePresetData;

        //states
        public ItemLocation Location_State { get; protected set; } = ItemLocation.World;

        //state info
        protected IS_BaseInfo _BaseInfo;

        #region Helpers
        [System.Serializable]
        public struct IS_BaseInfo
        {

        }
        #endregion

        #endregion

        private void OnDestroy()
        {
            OnItemDestroyed?.Invoke(this, new CSEventArgs.ItemEventArgs(this));
        }

        /// <summary>
        /// Virtual impl. Allows designer to implement custom disable routines for item types, should they be needed
        /// </summary>
        /// <returns></returns>
        public virtual bool RemoveFromWorldSpace()
        {
            bool successful = false;
            return successful;
        }
    }
}