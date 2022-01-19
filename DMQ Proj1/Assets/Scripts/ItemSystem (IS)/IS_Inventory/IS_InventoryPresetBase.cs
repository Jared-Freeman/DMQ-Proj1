using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ItemSystem
{
    [CreateAssetMenu(fileName = "INV_", menuName = "ScriptableObjects/Inventory Preset", order = 1)]
    public class IS_InventoryPresetBase : ScriptableObject
    {
        public static bool FLAG_DEBUG = false;

        public IS_InventoryPresetBaseOptions BaseOptions = new IS_InventoryPresetBaseOptions();


        [System.Serializable]
        public class IS_InventoryPresetBaseOptions
        {
            public Utils.TypeList<IS_ItemPresetBase> PermittedItemTypes = new Utils.TypeList<IS_ItemPresetBase>();

            [Min(0)]
            public int Capacity = 1;
        }


        //Item allowed
        public bool ItemAllowed(IS_ItemBase item)
        {
            return ItemAllowed(item.BasePresetData);
        }
        public bool ItemAllowed(IS_ItemPresetBase item)
        {
            if (BaseOptions.PermittedItemTypes.List.Count < 1 || BaseOptions.PermittedItemTypes.Contains(item))
            {
                if(FLAG_DEBUG) Debug.Log("ITEM PERMITTED");
                return true;
            }
            return false;
        }
    }
}