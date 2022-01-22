using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ItemSystem
{
    [CreateAssetMenu(fileName = "INV_", menuName = "ScriptableObjects/Inventory Preset", order = 1)]
    public class IS_InventoryPresetBase : ScriptableObject
    {
        public static bool s_FLAG_DEBUG = true;

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
                if(s_FLAG_DEBUG) Debug.Log("ITEM PERMITTED");
                return true;
            }
            else
            {
                //Here we must also check subclasses (just the impl i chose...yikes)
                foreach(var t in BaseOptions.PermittedItemTypes.List)
                {
                    if (item.GetType().IsSubclassOf(t))
                    {
                        if (s_FLAG_DEBUG) Debug.Log("subclass detect");
                        return true;
                    }
                }

                if (s_FLAG_DEBUG) Debug.Log("Not a subclass");
            }
            return false;
        }
    }
}