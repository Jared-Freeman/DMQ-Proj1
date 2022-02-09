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
    public class ItemAndInventoryEventArgs : System.EventArgs
    {
        public ItemSystem.IS_ItemBase Item;
        public ItemSystem.IS_InventoryBase Inventory;
        public ItemAndInventoryEventArgs(ItemSystem.IS_ItemBase item, ItemSystem.IS_InventoryBase inv)
        {
            Item = item;
            Inventory = inv;
        }
    }
}

namespace ItemSystem
{
    public enum ItemLocation { Inventory, World } //To distinguish between items in inventories or elsewhere
    public enum ItemPickupStyle { AddToInventory, AutoPickup };


    /// <summary>
    /// Base class for an instantiated item. Extending this should be paired with an IS_ItemPresetBase subclass impl!
    /// </summary>
    public abstract class IS_ItemBase : MonoBehaviour
    {
        #region Events
        public static event System.EventHandler<CSEventArgs.ItemEventArgs> OnItemDestroyed;
        public static event System.EventHandler<CSEventArgs.ItemEventArgs> OnItemRemovedFromWorldspace;
        public static event System.EventHandler<CSEventArgs.ItemEventArgs> OnItemAddedToWorldspace;

        //Local events to this instance
        public event System.EventHandler<CSEventArgs.ItemEventArgs> OnItemTransferred_Local;
        #endregion

        #region Members

        protected static bool s_FLAG_ITEM_DEBUG = true;

        //preset base
        [Header("WARNING: Preset must match item subclass!")]
        [SerializeField] private IS_ItemPresetBase _PresetData;

        /// <summary>
        /// Can override this property to cast to a more specific item preset
        /// </summary>
        public virtual IS_ItemPresetBase Preset
        {
            get
            {
                return _PresetData;
            }
            set
            {
                _PresetData = value;
            }
        }

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
            if(Preset == null)
            {
                Debug.LogError(ToString() + ": INVALID PRESET: Please check that you are using the proper preset subclass for this item!");
                Destroy(gameObject);
            }

            {
                //GameObject GO = new GameObject("Item Trigger Holder (Auto-Generated)");
                //GO.transform.parent = gameObject.transform;
                //GO.transform.localPosition = Vector3.zero;
                //GO.layer = 0; //Default

                var sc = gameObject.AddComponent<SphereCollider>();
                sc.radius = Preset.BaseOptions.PickupRadius;
                sc.isTrigger = true;
                gameObject.layer = 0; //Default

            }
        }

        private void OnTriggerEnter(Collider other)
        {
            {
                var inv = other.gameObject.GetComponent<Inventory_Player>(); //TODO: Other inventory handlers?
                if (inv != null)
                {
                    switch(Preset.BaseOptions.PickupStyle)
                    {
                        case ItemPickupStyle.AddToInventory:
                            inv.AddToItemsNearby(this);
                            break;
                        case ItemPickupStyle.AutoPickup:
                            if (inv.ItemAllowed(this)) inv.PickUpItem(this);
                            break;
                    }
                }
            }
        }
        private void OnTriggerExit(Collider other)
        {
            {
                var inv = other.gameObject.GetComponent<Inventory_Player>();
                if (inv != null)
                {
                    switch (Preset.BaseOptions.PickupStyle)
                    {
                        case ItemPickupStyle.AddToInventory:
                            inv.RemoveFromItemsNearby(this);
                            break;
                    }
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
        /// <returns>False if the itemw as already in inventory space (i.e. no "move" happened, such as being transferred from one inventory to another)</returns>
        public virtual bool AddItemToInventorySpace(IS_InventoryBase inv)
        {
            bool successful = false;

            if (Location_State != ItemLocation.Inventory)
            {
                successful = true;
                gameObject.SetActive(false);
                Location_State = ItemLocation.Inventory;

                //This ordering is PROBABLY important
                OnItemRemovedFromWorldspace?.Invoke(this, new CSEventArgs.ItemEventArgs(this));
                OnItemTransferred_Local?.Invoke(this, new CSEventArgs.ItemEventArgs(this));
                OnItemAddedToInventory(inv);
            }

            return successful;
        }
        /// <summary>
        /// Internal event handler. Can be overwritten
        /// </summary>
        /// <param name="inv"></param>
        protected virtual void OnItemAddedToInventory(IS_InventoryBase inv) { }


        public virtual bool AddItemToWorldSpace()
        {
            bool successful = true;

            gameObject.SetActive(true);
            Location_State = ItemLocation.World;

            //This ordering is PROBABLY important
            OnItemAddedToWorldspace?.Invoke(this, new CSEventArgs.ItemEventArgs(this));
            OnItemTransferred_Local?.Invoke(this, new CSEventArgs.ItemEventArgs(this));
            OnItemAddedToWorld();

            return successful;
        }
        /// <summary>
        /// Internal event handler. Can be overwritten
        /// </summary>
        protected virtual void OnItemAddedToWorld() { }

        /// <summary>
        /// Check if a given position is in the pickup sphere for this item
        /// </summary>
        /// <param name="pos">Position to be checked</param>
        /// <returns></returns>
        public bool InPickupRadius(Vector3 pos)
        {
            if((pos - transform.position).sqrMagnitude <= Preset.BaseOptions.PickupRadius * Preset.BaseOptions.PickupRadius)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Whether this item allows the inventory to accept it. 
        /// Virtual method can be overwritten.
        /// </summary>
        /// <param name="inv"></param>
        /// <returns></returns>
        public virtual bool InventoryAllowed(IS_InventoryBase inv)
        {
            return true;
        }
    }
}