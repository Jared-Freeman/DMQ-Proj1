using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_WeaponSlot : ItemSystem.IS_InventoryBase
{
    [System.Serializable]
    public struct InvWepSlotInfo
    {
        Item_Weapon CurrentWeapon;
    }

    protected override void Awake()
    {
        base.Awake();
        if(_Data.BaseOptions.Capacity > 1)
        {
            Debug.LogError("Weapon slots must have a Capacity BaseOption of 1! Errors may occur. Please check attached Preset.");
        }
    }


}
