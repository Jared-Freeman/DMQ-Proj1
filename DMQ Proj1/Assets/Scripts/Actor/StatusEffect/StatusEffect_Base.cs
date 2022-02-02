using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ActorSystem;

namespace ActorSystem.StatusEffect
{
    /// <summary>
    /// Defines a mutation to some ActorStats instance. 
    /// Contains multipliers AND adders to all stats.
    /// Useful for implementing mutations both positive and negative!
    /// </summary>
    public class StatusEffect_Base : ScriptableObject
    {
        public string Name;
        [Multiline]
        public string Description;

        [Header("Use the Modifier field for intended results.")]
        public ActorStatsData StatsModifiers;
    }
}