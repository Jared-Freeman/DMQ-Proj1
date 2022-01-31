using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ActorSystem
{
    [CreateAssetMenu(fileName = "Stats_", menuName = "Actor/Actor Stats Preset", order = 1)]
    public class ActorStatsPreset : ScriptableObject
    {
        /// <summary>
        /// Container for the Actor Stats Preset
        /// </summary>
        [System.Serializable]
        public struct ActorStatsPresetOptions
        {
            //might eventually want to perform a method call to alter these values (i.e. turning them into properties)
            //(i.e., changing max health reduces current health by (newmax - max))
            public float HpMax;
            public float EnergyMax;
        }
        public ActorStatsPresetOptions Options;

    }
}