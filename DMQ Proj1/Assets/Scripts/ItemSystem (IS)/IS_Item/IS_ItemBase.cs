using System.Collections;
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
        public static event System.EventHandler<CSEventArgs.ItemEventArgs> OnItemRemovedFromWorldspace;
        public static event System.EventHandler<CSEventArgs.ItemEventArgs> OnItemAddedToWorldspace;
        #endregion

        #region Members

        protected static bool s_FLAG_ITEM_DEBUG = true;

        //preset base
        [Header("Make sure to MATCH all IS_ItemPresetBase refs!!!")]
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

        protected virtual void Awake()
        {
            {
                GameObject GO = new GameObject("Item Trigger Holder (Auto-Generated)");

                var sc = GO.AddComponent<SphereCollider>();
                sc.radius = BasePresetData.BaseOptions.PickupRadius;
                sc.isTrigger = true;
                
                GO.transform.parent = gameObject.transform;
                GO.transform.localPosition = Vector3.zero;
                GO.layer = 0; //Default
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            {
                var inv = other.gameObject.GetComponent<Inventory_Player>();
                if (inv != null)
                {
                    inv.AddToItemsNearby(this);
                }
            }
        }
        private void OnTriggerExit(Collider other)
        {
            {
                var inv = other.gameObject.GetComponent<Inventory_Player>();
                if (inv != null)
                {
                    inv.RemoveFromItemsNearby(this);
                }
            }
        }

        public bool DestroyItem()
        {
            Destroy(gameObject);
            return true;
        }

        private void OnDestroy()
        {
            OnItemDestroyed?.Invoke(this, new CSEventArgs.ItemEventArgs(this));
        }

        /// <summary>
        /// Virtual impl. Allows designer to implement custom disable routines for item types, should they be needed
        /// </summary>
        /// <returns></returns>
        public virtual bool AddItemToInventorySpace()
        {
            bool successful = true;

            gameObject.SetActive(false);
            Location_State = ItemLocation.Inventory;
            OnItemRemovedFromWorldspace?.Invoke(this, new CSEventArgs.ItemEventArgs(this));

            return successful;
        }

        public virtual bool AddItemToWorldSpace()
        {
            bool successful = true;

            gameObject.SetActive(true);
            Location_State = ItemLocation.World;
            OnItemAddedToWorldspace?.Invoke(this, new CSEventArgs.ItemEventArgs(this));

            return successful;
        }

        /// <summary>
        /// Check if a given position is in the pickup sphere for this item
        /// </summary>
        /// <param name="pos">Position to be checked</param>
        /// <returns></returns>
        public bool InPickupRadius(Vector3 pos)
        {
            if((pos - transform.position).sqrMagnitude <= BasePresetData.BaseOptions.PickupRadius * BasePresetData.BaseOptions.PickupRadius)
            {
                return true;
            }
            return false;
        }
    }
}