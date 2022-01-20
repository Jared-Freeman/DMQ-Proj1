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

        private void Awake()
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
                    if(item.RemoveFromWorldSpace())
                    {
                        _ItemList.Add(item);
                        item.RemoveFromWorldSpace();
                        item.Location_State = ItemLocation.Inventory;
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

                item.Location_State = ItemLocation.World;

                _ItemList.Remove(item);

                return true;
            }
            return false;
        }

        /// <summary>
        /// Transfers an item from this container instance to another container instance
        /// </summary>
        /// <param name="item"></param>
        /// <param name="other_inventory">Other inventory to transfer this item into</param>
        /// <returns></returns>
        public bool TransferItem(IS_ItemBase item, IS_InventoryBase other_inventory)
        {
            throw new System.NotImplementedException();
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