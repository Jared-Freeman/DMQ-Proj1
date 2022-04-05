using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AbilitySystem;


namespace ActorSystem.AI
{
    /// <summary>
    /// Preset for <see cref="ActorAILogic_Charger"/>
    /// </summary>
    [CreateAssetMenu(fileName = "AI_", menuName = "Actor/AI Logic/Charger", order = 2)]
    public class ActorAILogic_Charger_Preset : ActorAI_Logic_PresetBase
    {
        /// <summary>
        /// Ability invoked when the charger cannot use its Charge ability, and is in range to deliver an attack.
        /// </summary>
        public AbilitySystem.AS_Ability_Base Ability_BasicAttack;
        /// <summary>
        /// Ability invoked when the charger collides with something during its charge state.
        /// </summary>
        public AbilitySystem.AS_Ability_Base Ability_OnChargeCollision;
        /// <summary>
        /// Ability invoked when the charger begins its lunge. 
        /// </summary>
        /// <remarks>
        /// Should probably be a physics impulse unless the designer is doing something fancy.
        /// </remarks>
        public AbilitySystem.AS_Ability_Base Ability_OnLungeBegin;

        public C_LungeOptions LungeOptions = new C_LungeOptions();
        public C_BasicAttackOptions BasicAttackOptions = new C_BasicAttackOptions();

        #region Helpers

        [System.Serializable]
        public class C_BasicAttackOptions
        {
            public float AnimationClipLength = 1f;
            public float AttackPause = 1f;

            public bool NeedLineOfSightToAttack = true;

            public float PrepareDistance = 5f;
            public float LosePrepareDistance = 8f;
        }

        [System.Serializable]
        public class C_LungeOptions
        {
            /// <summary>
            /// The length of the animation clip asset for lunging.
            /// </summary>
            /// <remarks>
            /// This info is needed in order to rescale the clip to match LungePause duration.
            /// </remarks>
            public float LungeAnimationClipLength = 1f;

            /// <summary>
            /// Distance at which standard basic attacks will instead be used.
            /// </summary>
            public float LungeTooCloseDistance = 6f;
            public float LungePrepareDistance = 8f;
            public float LungeLosePrepareDistance = 11f;

            /// <summary>
            /// When the agent goes below this value, the lunge will end.
            /// </summary>
            /// <remarks>
            /// This is an exit condition for when the agent hits a wall or otherwise slows down before LungeTimeout is called.
            /// </remarks>
            [Tooltip(
                "When the agent goes below this value, the lunge will end.\n"
                 + "This is an exit condition for when the agent hits a wall or otherwise slows down before LungeTimeout is called."
                )]
            public float MinimumSpeedToContinueLungeBehavior = 10f;

            /// <summary>
            /// Time to charge lunge attack
            /// </summary>
            public float LungePause = 1f;

            /// <summary>
            /// Time until AI mover reactivates upon casting lunge ability
            /// </summary>
            public float LungeTimeout = 2f;
        }

        #endregion
    }
}