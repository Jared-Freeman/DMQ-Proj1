using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AbilitySystem;

namespace ActorSystem.AI
{
    /// <summary>
    /// Charger Logic extends <see cref="ActorAI_Logic"/> and implements a behavior state machine where the AI seeks to charge towards its enemies.
    /// </summary>
    /// <remarks>
    /// By default, colliding ivokes its main ability.
    /// </remarks>
    public class ActorAILogic_Charger : ActorAI_Logic
    {
        #region Static Members

        public static float s_remainingDistanceTolerance = .2f;

        #endregion

        #region Members


        //public Utils.CooldownTracker AttackCooldown;
        //public AP2_ActorAction_AttackTarget AttackAction; //TODO: Change to ability

        protected AS_Ability_Instance_Base _AbilityInstance;


        protected override void ChooseNewTarget()
        {
            base.ChooseNewTarget();

            TurnTarget = CurrentTarget.transform;
        }

        /// <summary>
        /// override of Preset property that enforces correct subclassing
        /// </summary>
        public override ActorAI_Logic_PresetBase Preset
        {
            get
            {
                return base.Preset as ActorAILogic_Charger_Preset;
            }
            set
            {
                base.Preset = value as ActorAILogic_Charger_Preset;
            }
        }
        /// <summary>
        /// convenience property that wraps Preset
        /// </summary>
        public ActorAILogic_Charger_Preset C_Preset
        {
            get { return Preset as ActorAILogic_Charger_Preset; }
            protected set { Preset = value; }
        }


        #endregion



        #region Initialization
        protected override void Awake()
        {
            base.Awake();

            //AttackCooldown.InitializeCooldown();
            NavAgent.speed = Preset.Base.MovementSpeed;

            _AbilityInstance = C_Preset.Ability_OnChargeCollision.GetInstance(gameObject);
            //no test here because there may be use case for leaving _AbilityInstance null
        }

        protected override void Start()
        {
            base.Start();
        }
        #endregion
    }

}