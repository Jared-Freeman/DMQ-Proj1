using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ActorSystem;

namespace ClassSystem
{
    /// <summary>
    /// Preset for a Character Class
    /// </summary>
    [CreateAssetMenu(fileName = "Class_", menuName = "Player Data/Character Class", order = 1)]
    public class CharacterClass : ScriptableObject
    {
        /// <summary>
        /// Container for the rules this class follows, such as their set of useable stuff (perhaps), base stats, etc.
        /// </summary>
        [System.Serializable]
        public struct CharClassOptions
        {
            public ActorStatsPreset BaseStats;
        }

        public string ClassName;
        [Multiline]
        public string DisplayInfo;

        public CharClassOptions Options;


    }

}