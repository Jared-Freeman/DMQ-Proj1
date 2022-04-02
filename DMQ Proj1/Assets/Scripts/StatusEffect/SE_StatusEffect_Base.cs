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
        #region Events



        #endregion

        #region Members

        public enum DurationApplicationBehavior { ExtendDuration, ResetDuration, UseHighestDuration, None }

        public string Name;
        [Multiline] public string Description;
        public Sprite Icon;

        public SE_Preset_Settings Settings = new SE_Preset_Settings();


        #region Helper structs

        [System.Serializable]
        public class SE_Preset_Settings
        {
            public DurationApplicationBehavior DurationExtensionType = DurationApplicationBehavior.UseHighestDuration;
            public SE_Defaults Defaults = new SE_Defaults();
            public SE_Flags Flags;
            public SE_EffectSettings Effects;
            public ActorStatsData StatsModifiers = new ActorStatsData();
        }

        [System.Serializable]
        public struct SE_Flags
        {
            public bool InfiniteDuration;
        }

        [System.Serializable]
        public class SE_Defaults
        {
            [Min(0f)]
            public float Duration = 1;
            [Min(0)]
            public int MaxStacks = 1;
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

            [Header("Effect Destroyed occurs when a stack is added to the Status Effect.")]
            public Effect_Base Effect_StackAdded;
        }

        #endregion

        #endregion

        /// <summary>
        /// Creates or gets status effect instance using this preset's data.
        /// </summary>
        /// <param name="instance_holder">Gameobject the status effect instance attaches to</param>
        /// <returns>The newly created status effect component, or the existing instance on this object</returns>
        /// <remarks>
        /// Is not guaranteed to make a NEW instance, but will always return a <see cref="SE_StatusEffect_Instance"/>
        /// </remarks>
        // TODO: Create a method to pass in context!
        public SE_StatusEffect_Instance CreateInstance(GameObject instance_holder, DurationApplicationBehavior durationBehavior = DurationApplicationBehavior.ResetDuration)
        {
            var sfx = instance_holder.GetComponents<SE_StatusEffect_Instance>();
            
            //find existing instance, should it exist.
            SE_StatusEffect_Instance inst = null;
            foreach (var f in sfx)
            {
                if(f.Preset == this)
                {
                    inst = f;

                    switch(durationBehavior)
                    {
                        case DurationApplicationBehavior.ExtendDuration:
                            inst.RemainingDuration += Settings.Defaults.Duration;
                            break;


                        //currently no fancy impl exists for UseHighest.
                        case DurationApplicationBehavior.UseHighestDuration:
                        case DurationApplicationBehavior.ResetDuration:
                            inst.RemainingDuration = Settings.Defaults.Duration;
                            break;

                        case DurationApplicationBehavior.None:
                            break;

                        default:
                            Debug.LogError("No impl found for durationBehavior! Does one exist?");
                            break;
                    }
                }
            }

            if(inst == null)
            {
                inst = instance_holder.AddComponent<SE_StatusEffect_Instance>();
                inst.Preset = this;
            }

            return inst;
        }
    }
}