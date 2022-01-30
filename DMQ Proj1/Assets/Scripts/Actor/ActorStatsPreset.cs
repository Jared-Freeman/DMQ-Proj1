using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ActorSystem
{
    [CreateAssetMenu(fileName = "Class_", menuName = "ScriptableObjects/Actor Stats Preset", order = 1)]
    public class ActorStatsPreset : ScriptableObject
    {
        /// <summary>
        /// Container for the Actor Stats Preset
        /// </summary>
        [System.Serializable]
        public struct ActorStatsPresetOptions
        {

        }
        public ActorStatsPresetOptions Options;

    }
}