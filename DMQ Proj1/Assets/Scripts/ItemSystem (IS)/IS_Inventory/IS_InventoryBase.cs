using System;
using System.Collections;
using System.Collections.Generic;
using CSEventArgs;
using UnityEngine;

namespace ItemSystem
{
    /// <summary>
    /// Instantiated Container for IS_Item's.
    /// Contains methods to access item data, and move items to other containers
    /// </summary>
    public abstract class IS_InventoryBase : MonoBehaviour
    {
        #region Members

        protected bool s_FLAG_DEBUG = true;

        [SerializeField] protected IS_InventoryPresetBase _Data;
        protected IS_Inv_StateInfo _Info;

        //Internal
        protected List<IS_ItemBase> _ItemList = new List<IS_ItemBase>();

        //Outer facing (dont modify this)
        public IReadOnlyCollection<IS_ItemBase> Items
        {
            get
            {
                return _ItemList.AsReadOnly();
            }
        }

        #region Helpers
        [System.Serializable]
        public struct IS_Inv_StateInfo
        {

        }
        #endregion

        #endregion


        #region Initialization

        protected virtual void Awake()
        {
            if (_Data == null)
            {
                Debug.LogError(ToString() + ": No Preset found! Destroying this");
                Destroy(this);
            }


        }

        private void OnEnable()
        {
            ValidateItemList();

            IS_ItemBase.OnItemDestroyed += CheckItemDestroyed;
        }
        
        private void OnDisable()
        {
            IS_ItemBase.OnItemDestroyed -= CheckItemDestroyed;
        }


        #endregion



        /// <summary>
        /// Place an item from world space into this container
        /// </summary>
        /// <param name="item">Item to be added to this inventory</param>
        /// <returns></returns>
        public bool PickUpItem(IS_ItemBase item)
        {
            if (item.Location_State == ItemLocation.World)
            {
                //TODO: If we implement a more complex cost (i.e. weight) this needs to be altered

                //Must have room left, and Item type must be allowed to be in this container
                if (_ItemList.Count < _Data.BaseOptions.Capacity && _Data.ItemAllowed(item))
                {
                    if(item.AddItemToInventorySpace())
                    {
                        _ItemList.Add(item);
                        item.AddItemToInventorySpace();
                        return true;
                    }

                }
            }
            else
            {
                Debug.LogError("Attempting to pick up an item not in world space!");
            }
            

            return false;
        }


        /// <summary>
        /// Places an item in world space
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool DropItem(IS_ItemBase item, Vector3 Location = default, Quaternion Rotation = default)
        {
            if(_ItemList.Contains(item) && item.AddItemToWorldSpace())
            {
                if (Location != default)
                {
                    item.transform.position = Location;
                }
                if(Rotation != default)
                {
                    item.transform.rotation = Rotation;
                }

                _ItemList.Remove(item);

                return true;
            }
            return false;
        }

        /// <summary>
        /// Transfers an item from this container instance to another container instance. Returns false if the attempt fails (i.e. no capacity, type not allowed)
        /// </summary>
        /// <param name="item">Item to transfer</param>
        /// <param name="other_inventory">Other inventory to transfer this item into</param>
        /// <returns></returns>
        public virtual bool TransferItem(IS_ItemBase item, IS_InventoryBase other_inventory)
        {
            if (!_ItemList.Contains(item))
            {
                if (s_FLAG_DEBUG) Debug.Log("item not found in itemlist");

                return false; //invalid -- item not in this inventory

            }

            if (other_inventory.ReceiveItemFromInventory(item))
            {
                if (s_FLAG_DEBUG) Debug.Log("Item Received");

                _ItemList.Remove(item);
                return true;
            }
            else
            {
                if (s_FLAG_DEBUG) Debug.Log("Item Not Received");
            }

            return false;
        }

        //Please note that this can be extended
        public virtual bool ReceiveItemFromInventory(IS_ItemBase item)
        {
            if(_ItemList.Count < _Data.BaseOptions.Capacity && _Data.ItemAllowed(item))
            {
                if (s_FLAG_DEBUG) Debug.Log("Base Receive evaluated to true");

                _ItemList.Add(item);
                return true;
            }


            if(s_FLAG_DEBUG) Debug.Log("Capacity error: " + !(_ItemList.Count < _Data.BaseOptions.Capacity));
            if (s_FLAG_DEBUG) Debug.Log("Item Return error: " + !(_Data.ItemAllowed(item)));
            return false;
        }

        #region Utility Methods

        private void CheckItemDestroyed(object sender, CSEventArgs.ItemEventArgs args)
        {
            if (_ItemList.Contains(args.Item))
            {
                _ItemList.Remove(args.Item);
            }
        }

        private void ValidateItemList()
        {
            var ValidatedList = new List<IS_ItemBase>();

            foreach (var i in _ItemList)
            {
                if (i != null)
                {
                    ValidatedList.Add(i);
                }
            }

            _ItemList = ValidatedList;
        }
        #endregion
    }
}