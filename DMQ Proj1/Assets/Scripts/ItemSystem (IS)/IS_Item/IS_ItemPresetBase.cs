using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ItemSystem
{
    public abstract class IS_ItemPresetBase : ScriptableObject
    {
        [System.Serializable]
        public struct IS_Options
        {
            public float PickupRadius;
        }

        public IS_Options BaseOptions;
    }
}