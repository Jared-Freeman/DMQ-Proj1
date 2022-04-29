using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ItemSystem
{
    public abstract class IS_ItemPresetBase : ScriptableObject
    {
        [System.Serializable]
        public class IS_Options
        {
            /// <summary>
            /// Radius where this item can be picked up. Should be larger than the collider radius, should one exist
            /// </summary>
            public float PickupRadius = 2f;
            /// <summary>
            /// Controls how this item is picked up
            /// </summary>
            public ItemPickupStyle PickupStyle = ItemPickupStyle.AddToInventory;
            /// <summary>
            /// How much inventory capacity does this item consume?
            /// </summary>
            public int CapacityCost = 1;

            public AbilitySystem.AS_Ability_Base Ability_OnItemPickedUp;
        }

        public IS_Options BaseOptions;
    }
}