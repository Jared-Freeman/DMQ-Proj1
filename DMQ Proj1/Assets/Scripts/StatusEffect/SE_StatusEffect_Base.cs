using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using EffectTree;
using ActorSystem;


namespace ActorSystem.StatusEffect
{
    /// <summary>
    /// A preset containing the info to impl any status effect
    /// </summary>
    [CreateAssetMenu(fileName = "STATUS_", menuName = "Actor/Status Effect", order = 1)]
    public class SE_StatusEffect_Base : ScriptableObject
    {
        #region Members

        public string Name;
        [Multiline] public string Description;
        public Sprite Icon;

        public SE_Preset_Settings Settings;


        #region Helper structs

        [System.Serializable]
        public struct SE_Preset_Settings
        {
            public SE_Defaults Defaults;
            public SE_Flags Flags;
            public SE_EffectSettings Effects;
            public ActorStatsData StatsModifiers;
        }

        [System.Serializable]
        public struct SE_Flags
        {
            public bool InfiniteDuration;
        }

        [System.Serializable]
        public struct SE_Defaults
        {
            [Min(0f)]
            public float Duration;
        }


        [System.Serializable]
        public struct SE_EffectSettings
        {
            [Header("Effect Begin occurs when status is added to the target Actor.")]
            public Effect_Base Effect_Begin;

            [Header("Effect Periodic every PeriodicEffectInterval seconds.")]
            public Effect_Base Effect_Periodic;
            //surely there isn't a reason to perform an effect more than this right???
            [Min(.025f)]
            public float PeriodicEffectInterval;

            [Header("Effect End occurs when the effect completes its FULL DURATION.")]
            public Effect_Base Effect_End;

            [Header("Effect Destroyed occurs when the effect is removed (regardless of duration).")]
            public Effect_Base Effect_Destroyed;
        }

        #endregion

        #endregion

        /// <summary>
        /// Creates a new status effect instance using this preset's data.
        /// </summary>
        /// <param name="instance_holder">Gameobject to attach the new component to</param>
        /// <returns>The newly created status effect component</returns>
        // TODO: Create a method to pass in context!
        public SE_StatusEffect_Instance CreateInstance(GameObject instance_holder)
        {
            var inst = instance_holder.AddComponent<SE_StatusEffect_Instance>();
            inst.Preset = this;

            return inst;
        }
    }
}