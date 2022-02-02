using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ActorSystem;

namespace ActorSystem.StatusEffect
{
    /// <summary>
    /// Defines a PERSISTENT mutation to some ActorStats instance. 
    /// [NYI] Can be disabled/enabled OR destroyed.
    /// Contains multipliers AND adders to all stats.
    /// Useful for implementing mutations both positive and negative!
    /// </summary>
    public class StatusEffect_Base : ScriptableObject
    {
        public string Name;
        [Multiline]
        public string Description;

        /// <summary>
        /// The Stat Modifier applied to the Actor 
        /// </summary>
        [Header("Use the Modifier field for intended results.")]
        public ActorStatsData StatModifiers;

        public SFX_Opt Options;

        [System.Serializable]
        public struct SFX_Opt
        {

        }
    }
}