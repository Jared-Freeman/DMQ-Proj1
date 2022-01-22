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
            public float PickupRadius = 2f;
        }

        public IS_Options BaseOptions;
    }
}