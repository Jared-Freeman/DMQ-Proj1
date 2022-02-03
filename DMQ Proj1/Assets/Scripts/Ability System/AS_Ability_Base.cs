using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem
{
    /// <summary>
    /// An Ability is a triggerable start to an effect tree. 
    /// For instance, by listening to player IO we can execute Ability's.
    /// Abilities include settings that define how they may be used.
    /// 
    /// The base class will simply cast the effect if conditions (e.g., cooldown, settings, conditions List<>) are true
    /// </summary>
    [CreateAssetMenu(fileName = "A_", menuName = "Ability/Standard Ability", order = 2)]
    public class AS_Ability_Base : ScriptableObject
    {
        public Utils.CooldownTracker Cooldown;
        public AS_AbilCastSettings CastSettings;
        public EffectTree.Condition.ConditionList Conditions = new EffectTree.Condition.ConditionList();
        public Sprite IconImage; //TODO: Convert this into a UI element or something

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
        /// Creates a NEW, component instance of this ability type and attaches it to the ability_owner
        /// </summary>
        /// <returns>A newly-constructed instance of this ability type.</returns>
        public virtual AS_Ability_Instance_Base GetInstance(GameObject ability_owner)
        {
            var instance = ability_owner.AddComponent<AS_Ability_Instance_Base>();

            if (instance == null) Debug.LogError(ToString() + ": instance is null!");

            instance.Settings = this;

            return instance;
        }
    }
}
