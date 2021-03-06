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
        public AS_AbilEffectTreeEvents EffectEvents;
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
        [System.Serializable]
        public struct AS_AbilEffectTreeEvents
        {
            /// <summary>
            /// Invoked when the abilitiy comes back off-cooldown
            /// </summary>
            public EffectTree.Effect_Base Effect_CooldownAvailable;
            /// <summary>
            /// Invoked when the player attempts to cast while the Ability instance is on cooldown
            /// </summary>
            public EffectTree.Effect_Base Effect_CastWhileOnCooldown;

            //public EffectTree.Effect_Base Effect_;
        }

        /// <summary>
        /// Creates a NEW, component instance of this ability type and attaches it to the ability_owner
        /// </summary>
        /// <returns>A newly-constructed instance of this ability type.</returns>
        public virtual AS_Ability_Instance_Base GetInstance(GameObject ability_owner)
        {
            var instance = ability_owner.AddComponent<AS_Ability_Instance_Base>();
            Actor actorOwner = ability_owner.GetComponent<Actor>();

            if (instance == null) Debug.LogError(ToString() + ": instance is null!");
            else if(actorOwner != null)
            {
                instance.Owner = actorOwner;
            }

            instance.Settings = this;

            return instance;
        }
    }
}
