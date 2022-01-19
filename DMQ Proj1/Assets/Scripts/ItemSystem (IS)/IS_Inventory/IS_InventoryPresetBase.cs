using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ItemSystem
{
    [CreateAssetMenu(fileName = "INV_", menuName = "ScriptableObjects/Inventory Preset", order = 1)]
    public class IS_InventoryPresetBase : ScriptableObject
    {
        public IS_InventoryPresetBaseOptions BaseOptions;


        [System.Serializable]
        public struct IS_InventoryPresetBaseOptions
        {
            public Utils.TypeList<IS_ItemPresetBase> PermittedItemTypes;

            [Min(0)]
            public int Capacity;
        }


        //Item allowed
        public bool ItemAllowed(IS_ItemBase item)
        {
            if (BaseOptions.PermittedItemTypes.Contains(item.BasePresetData)) return true;
            return false;
        }
        public bool ItemAllowed(IS_ItemPresetBase item)
        {
            if (BaseOptions.PermittedItemTypes.Contains(item)) return true;
            return false;
        }
    }
}