using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ActorSystem;

namespace ClassSystem
{
    /// <summary>
    /// Preset for a Character Class. Please note this is different than ActorSystem ActorStatsPreset.
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
            public GameObject PlayerPrefab;
        }

        public string ClassName;
        [Multiline]
        public string DisplayInfo;

        public CharClassOptions Options;

        /// <summary>
        /// This method assume the PlayerPrefab has the appropriate components attached!
        /// </summary>
        /// <returns>A GameObject with an instantiated player prefab of this class type</returns>
        public GameObject InstantiatePlayerActor()
        {
            if (Options.PlayerPrefab == null) return null;

            GameObject g = Instantiate(Options.PlayerPrefab);

            return g;
        }
    }

}