using System;
using System.Collections;
using System.Collections.Generic;
using CSEventArgs;
using UnityEngine;
using UnityEngine.InputSystem;
using ItemSystem.Weapons;

public class InventoryEventArgs : System.EventArgs
{
    public InventoryEventArgs(GameObject o,int slot, Item_Weapon_ClassSpecific wep)
    {
        obj = o;
        weaponSlot = slot;
        weapon = wep;
    }
    public GameObject obj;
    public int weaponSlot;
    public Item_Weapon_ClassSpecific weapon;
}


//TODO: Interact may need to be buffered/queued somehow (to prevent Interact from doing multiple things)
/// <summary>
/// Player inventory instance. Grants players agency over picking up items using player input. 
/// Maintains WeaponSlot subinventories and acces to Current Weapon reference.
/// </summary>
public class Inventory_Player : ItemSystem.IS_InventoryBase
{
    public static event System.EventHandler<InventoryEventArgs> OnWeaponChanged;
    #region Members

    public ItemSystem.Weapons.Item_WeaponBase CurrentWeapon 
    { get
        {
            if (Info.EquippedWeaponIndex < 0 || Info.EquippedWeaponIndex >= _WeaponSlots.Count) //bounds
            {
                return null;
            }
            else
            {
                return _WeaponSlots[Info.EquippedWeaponIndex].Weapon;
            }
        } 
    }
    /// <summary>
    /// More specific weapon property than CurrentWeapon
    /// </summary>
    public ItemSystem.Weapons.Item_Weapon_ClassSpecific CurrentClassWeapon
    {
        get
        {
            if (CurrentWeapon == null) return null;
            return CurrentWeapon as ItemSystem.Weapons.Item_Weapon_ClassSpecific;
        }
    }

    PlayerInputHost InputHost { get; set; }
    PlayerInput Input { get { return InputHost.CurrentPlayerInput; } }
    PlayerControls controls;

    [SerializeField] protected List<Inventory_WeaponSlot> _WeaponSlots = new List<Inventory_WeaponSlot>();

    [SerializeField] protected PlayerInvInfo Info;

    [Tooltip("No reference here will use the Transform of THIS Gameobject")]
    [SerializeField] protected Transform _PickupTransform;

    #region Helpers
    [System.Serializable]
    public struct PlayerInvInfo
    {
        public List<ItemSystem.IS_ItemBase> ItemsNearby;
        public int EquippedWeaponIndex; // values < 0 imply no weapon is equipped!
    }
    #endregion
    #endregion


    /// <summary>
    /// Transform where Pickup checks are done for this Player Inventory
    /// </summary>
    public Transform PickupLocation { get { return _PickupTransform; } }


    public bool ItemNearby
    {
        get
        {
            if (Info.ItemsNearby.Count > 0) return true;
            return false;
        }
    }

    protected override void Awake()
    {
        base.Awake();

        if(_PickupTransform == null)
            _PickupTransform = gameObject.transform; //can change if needed

        Info.ItemsNearby = new List<ItemSystem.IS_ItemBase>();

        //Game currently set to include 2 weapon slots... Maybe we can make this nicer later or something idk
        int wepSlotsNeeded = 2;

        var attachedSlots = gameObject.GetComponents<Inventory_WeaponSlot>();
        foreach(var w in attachedSlots)
        {
            _WeaponSlots.Add(w);
        }

        if (_WeaponSlots.Count != wepSlotsNeeded) Debug.LogError("Weapon Slots not set to " + wepSlotsNeeded + "! Could be errors");
        foreach(var w in _WeaponSlots)
        {
            if (w == null) Debug.LogError("Weaponslot inventory not set! Please Check that all Weaponslots have an inventory reference.");
        }

        Info.EquippedWeaponIndex = -1; //No weapon equipped
    }

    protected override void Start()
    {
        base.Start();

        InitInput();
    }

    private void OnEnable()
    {
        ItemSystem.IS_ItemBase.OnItemRemovedFromWorldspace += CheckRemoveFromItemsNearby;
    }
    private void OnDisable()
    {
        ItemSystem.IS_ItemBase.OnItemRemovedFromWorldspace -= CheckRemoveFromItemsNearby;
    }

    private void CheckRemoveFromItemsNearby(object sender, ItemEventArgs e)
    {
        RemoveFromItemsNearby(e.Item);
    }


    /// <summary>
    /// This function is called in Awake(), and creates controls + registers all the events that may occur due to player input
    /// </summary>
    private void InitInput()
    {
        InputHost = GetComponent<PlayerInputHost>();
        if(InputHost == null)
        {
            Debug.LogError(ToString() + ": No Input Host component found! Destroying.");
            Destroy(this);
        }

        controls = new PlayerControls();

        //set up action map
        if (Input.currentControlScheme == controls.MouseAndKeyboardScheme.name)
        {
            Input.SwitchCurrentActionMap(controls.MouseAndKeyboardScheme.name);
        }
        else if (Input.currentControlScheme == controls.GamepadScheme.name)
        {
            Input.SwitchCurrentActionMap(controls.GamepadScheme.name);
        }

        //please someone find a better way to do this (and retain multiplayer functionality)
        Input.onActionTriggered += ctx =>
        {
            ////MOUSE AND KEYBOARD EVENTS REGISTER //////////////////////////////////////
            if (ctx.action.actionMap.name == controls.MouseAndKeyboardScheme.name)
            {
                if (ctx.performed)
                {
                    //MnK
                    if (ctx.action.name == controls.MouseAndKeyboard.Interact.name)
                    {
                        PickUpNearestItem();
                    }
                    else if (ctx.action.name == controls.MouseAndKeyboard.Jump.name)
                    {
                        DropFirstItem();
                    }
                    else if (ctx.action.name == controls.MouseAndKeyboard.Wepon1Equip.name)
                    {
                        EquipWeaponSlot(0);
                    }
                    else if (ctx.action.name == controls.MouseAndKeyboard.Wepon2Equip.name)
                    {
                        EquipWeaponSlot(1);
                    }

                }


                else if (ctx.canceled)
                {
                    //MnK
                    //if (ctx.action.name == controls.MouseAndKeyboard.Movement.name) InputMap = Vector2.zero;
                }
            }



            ////GAMEPAD EVENTS REGISTER //////////////////////////////////////
            else if (ctx.action.actionMap.name == controls.GamepadScheme.name)
            {
                if (ctx.performed)
                {
                    //Gamepad
                    if (ctx.action.name == controls.Gamepad.Interact.name)
                    {
                        PickUpNearestItem();
                    }
                    else if (ctx.action.name == controls.Gamepad.Jump.name)
                    {
                        DropFirstItem();
                    }
                    else if (ctx.action.name == controls.Gamepad.Wepon1Equip.name)
                    {
                        EquipWeaponSlot(0);
                    }
                    else if (ctx.action.name == controls.Gamepad.Wepon2Equip.name)
                    {
                        EquipWeaponSlot(1);
                    }



                }

                else if (ctx.canceled)
                {
                    //Gamepad
                    //if (ctx.action.name == controls.Gamepad.Movement.name) InputMap = Vector2.zero;
                }
            }
        };


    }//end InitInput()

    /// <summary>
    /// Equips Weapon in Weaponslot[ index ]
    /// </summary>
    /// <param name="index">weapon slot index to equip. Set to -1 to dequip</param>
    private void EquipWeaponSlot(int index)
    {
        if(index < 0)
        {
            //dequip
            Info.EquippedWeaponIndex = -1;
        }
        else if(index < _WeaponSlots.Count)
        {
            //No item in the weapon slot, or duplicate equip request
            if (_WeaponSlots[index].Items.Count < 1 || index == Info.EquippedWeaponIndex) return;

            //otherwise we equip
            Debug.Log("WEAPON EQUIPPED @index: " + index);
            Info.EquippedWeaponIndex = index;
            Item_Weapon_ClassSpecific currentWeapon = (Item_Weapon_ClassSpecific)_WeaponSlots[index].Weapon;
            OnWeaponChanged.Invoke(this, new InventoryEventArgs(this.gameObject,index,currentWeapon));
        }
        else
        {
            Debug.LogError(ToString() + ": Weapon Slot index out of range! Aborting Equip");
        }
    }


    //TODO: Make something better than this
    /// <summary>
    /// Drops first item in inventory. Searches Weapon Slot's after player inventory items are depleted
    /// </summary>
    private void DropFirstItem()
    {
        if(_ItemList.Count > 0)
        {
            if (DropItem(_ItemList[0], _PickupTransform.position))
            {
                //nothing to do here yet
            }
        }
        else
        {
            foreach (var w in _WeaponSlots)
            {
                if (w.Weapon != null)
                {
                    w.DropItem(w.Weapon, _PickupTransform.position);
                    return;
                }
            }
        }
    }

    /// <summary>
    /// Displays the item in ItemsNearby closest to the gameobject's position
    /// </summary>
    private void DisplayNearestItem()
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Add an item to this player inventory's ItemsNearby list
    /// </summary>
    /// <param name="item">Item to be added</param>
    public void AddToItemsNearby(ItemSystem.IS_ItemBase item)
    {
        if(!Info.ItemsNearby.Contains(item))
        {
            Info.ItemsNearby.Add(item);
        }
    }

    /// <summary>
    /// Remove an item from this player inventory's ItemsNearby list, if it exists in the list
    /// </summary>
    /// <param name="item">Item to be removed</param>
    public void RemoveFromItemsNearby(ItemSystem.IS_ItemBase item)
    {
        if (Info.ItemsNearby.Contains(item))
        {
            Info.ItemsNearby.Remove(item);
        }
    }


    /// <summary>
    /// Validates that a pickuppable item exists, then finds item in ItemsNearby that is closest to player, and picks it up
    /// </summary>
    private void PickUpNearestItem()
    {
        if (ItemNearby)
        {
            ItemSystem.IS_ItemBase nearest = Info.ItemsNearby[0];
            float ClosestMagnitudeSquared = Mathf.Infinity;
            float CurMag;

            foreach(var i in Info.ItemsNearby)
            {
                CurMag = (i.gameObject.transform.position - _PickupTransform.position).sqrMagnitude;
                if (CurMag < ClosestMagnitudeSquared)
                {
                    ClosestMagnitudeSquared = CurMag;
                    nearest = i;
                }
            }

            //We need to remove the newly-disabled ref from all lists
            if (PickUpItem(nearest))
            {
                //Debug.Log("Item picked");

                //Debug.Log("Slots: " + _WeaponSlots.Count);

                //if(TransferItem(nearest, _WeaponSlots[0]))
                //{
                //    Debug.Log("Item transferred to slot 0!");
                //}

                //Auto-adds a weapon to an empty slot, if one exists
                foreach (var w in _WeaponSlots)
                {
                    if (TransferItem(nearest, w))
                    {
                        //cool it worked
                        //Debug.Log("Item transferred to slot!");
                        break;
                    }
                }
            }
        }
    }
}
