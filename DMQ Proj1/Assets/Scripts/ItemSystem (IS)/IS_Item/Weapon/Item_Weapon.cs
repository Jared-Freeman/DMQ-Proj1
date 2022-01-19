using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Weapon : ItemSystem.IS_ItemBase
{
    [System.Serializable]
    public struct AddedOptions
    {

    }

    public AddedOptions BaseWeaponOptions;
    public ItemSystem.IS_WeaponPreset BaseWeaponData;

    protected override void Awake()
    {
        base.Awake();

        if (BaseWeaponData == null)
        {
            Debug.LogError(ToString() + ": No Data attached! Destroying");
            Destroy(gameObject);
        }
    }

    //This impl is the fairly standard way of overriding the base method
    /// <summary>
    /// Disables the gameObject and returns trues
    /// </summary>
    /// <returns></returns>
    public override bool RemoveFromWorldSpace()
    {
        //base is sufficient for this class
        base.RemoveFromWorldSpace();

        return true;
    }
}
