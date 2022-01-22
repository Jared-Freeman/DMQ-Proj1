using ItemSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Container for a SINGLE weapon. Inventory_WeaponSlot instance(s) should be accompanied by a Inventory_Player instance.
/// </summary>
public class Inventory_WeaponSlot : ItemSystem.IS_InventoryBase
{
    [System.Serializable]
    public struct InvWepSlotInfo
    {
        public ItemSystem.Weapons.Item_WeaponBase _CurrentWeapon;
    }

    protected InvWepSlotInfo Info;

    public ItemSystem.Weapons.Item_WeaponBase Weapon { get { return Info._CurrentWeapon; } }
    public bool SlotEmpty { get { return (Items.Count <= _Data.BaseOptions.Capacity); } }

    protected override void Awake()
    {
        base.Awake();
        if(_Data.BaseOptions.Capacity > 1)
        {
            Debug.LogError("Weapon slots must have a Capacity BaseOption of 1! Errors may occur. Please check attached Preset.");
        }

        //Note: this stuff will be called per-instance... not the best idea but whatever
        _Data.BaseOptions.PermittedItemTypes.Clear();
        _Data.BaseOptions.PermittedItemTypes.Add<IS_WeaponPreset>(); //set -- only weapons are allowed for this inventory preset.
    }

    public override bool ReceiveItemFromInventory(IS_ItemBase item)
    {
        if( base.ReceiveItemFromInventory(item))
        {
            UpdateCurrentWeapon();
            return true;
        }
        return false;
    }

    //Contains a somewhat annoying cast here. Dunno how else to do this tho.
    private void UpdateCurrentWeapon()
    {
        Info._CurrentWeapon = (ItemSystem.Weapons.Item_WeaponBase)_ItemList[0]; //explicit cast. Items in this list are guaranteed to be Weapon's due to Awake()
    }
}
