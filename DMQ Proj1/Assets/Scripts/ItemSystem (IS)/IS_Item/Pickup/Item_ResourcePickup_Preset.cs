using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ItemSystem
{
    [CreateAssetMenu(fileName = "PICKUP_", menuName = "ScriptableObjects/Items/Pickup", order = 2)]
    public class Item_ResourcePickup_Preset : IS_ItemPresetBase
    {
        public List<Team> List_WhoCanPickUp = new List<Team>();

        public EffectTree.Effect_Base PickupEffect;
    }

}