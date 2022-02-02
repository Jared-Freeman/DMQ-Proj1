using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem
{
    /// <summary>
    /// An Ability is a triggerable start to an effect tree. 
    /// For instance, by listening to player IO we can execute Ability's.
    /// Abilities include settings that define how they may be used
    /// </summary>
    public abstract class AS_Ability_Base : ScriptableObject
    {
        public Utils.CooldownTracker Cooldown;
        public AS_AbilCastSettings CastSettings;

        /// <summary>
        /// The effect to invoke when this ability is cast.
        /// </summary>
        public EffectTree.Effect_Base Effect; 

        [System.Serializable]
        public struct AS_AbilCastSettings
        {
            //flags
            public bool CanMoveWhileCasting;
            public bool CanCancelCast;

            //durations
            public float Time_PrepareAbil;
            public float Time_CastAbil;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>A newly-constructed instance of this ability type.</returns>
        public virtual AS_Ability_Instance_Base GetInstance(GameObject ability_owner)
        {
            return null;
        }
    }
}
