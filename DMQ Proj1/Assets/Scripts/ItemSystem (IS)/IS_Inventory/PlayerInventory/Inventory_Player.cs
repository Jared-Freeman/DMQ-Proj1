using System;
using System.Collections;
using System.Collections.Generic;
using CSEventArgs;
using UnityEngine;
using UnityEngine.InputSystem;

//TODO: Interact may need to be buffered/queued somehow (to prevent Interact from doing multiple things)
/// <summary>
/// Player inventory instance. Grants players agency over picking up items using player input
/// </summary>
public class Inventory_Player : ItemSystem.IS_InventoryBase
{
    [System.Serializable]
    public struct PlayerInvInfo
    {
        public List<ItemSystem.IS_ItemBase> ItemsNearby;
    }

    PlayerInput Input;
    PlayerControls controls;

    [SerializeField] protected PlayerInvInfo Info;

    public Transform PickupTransform { get; protected set; }


    public bool ItemNearby
    {
        get
        {
            if (Info.ItemsNearby.Count > 0) return true;
            return false;
        }
    }

    private void Awake()
    {
        PickupTransform = gameObject.transform; //can change if needed

        Info.ItemsNearby = new List<ItemSystem.IS_ItemBase>();

        InitInput();
    }

    private void OnEnable()
    {
        ItemSystem.IS_ItemBase.OnItemRemovedFromWorldspace += DispatchToCheckItemsNearby;
    }
    private void OnDisable()
    {
        ItemSystem.IS_ItemBase.OnItemRemovedFromWorldspace -= DispatchToCheckItemsNearby;
    }

    private void DispatchToCheckItemsNearby(object sender, ItemEventArgs e)
    {
        RemoveFromItemsNearby(e.Item);
    }


    /// <summary>
    /// This function is called in Awake(), and creates controls + registers all the events that may occur due to player input
    /// </summary>
    private void InitInput()
    {
        Input = GetComponent<PlayerInput>();
        if(Input == null)
        {
            Debug.LogError(ToString() + ": No PlayerInput component found! Destroying.");
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
                CurMag = (i.gameObject.transform.position - PickupTransform.position).sqrMagnitude;
                if (CurMag < ClosestMagnitudeSquared)
                {
                    ClosestMagnitudeSquared = CurMag;
                    nearest = i;
                }
            }

            //We need to remove the newly-disabled ref from all lists
            if (PickUpItem(nearest))
            {
            }
        }
    }
}
