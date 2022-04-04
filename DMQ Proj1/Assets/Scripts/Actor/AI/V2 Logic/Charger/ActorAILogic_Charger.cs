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

        protected AS_Ability_Instance_Base _AbilityInstance_OnCollideEnter;
        protected AS_Ability_Instance_Base _AbilityInstance_OnLungeBegin;


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

            _AbilityInstance_OnCollideEnter = C_Preset.Ability_OnChargeCollision.GetInstance(gameObject);
            _AbilityInstance_OnLungeBegin = C_Preset.Ability_OnLungeBegin.GetInstance(gameObject);
            //no test here because there may be use case for leaving _AbilityInstance's null
        }

        #endregion

        #region Update

        protected override void UpdateAI()
        {
            base.UpdateAI();

            switch (CurrentState)
            {
                case ActorAILogic_State.Idle:
                    SearchForTargets();
                    break;

                case ActorAILogic_State.Chasing:
                    ChaseCurrentTarget();
                    break;

                case ActorAILogic_State.ChargingAttack:
                    WaitToLunge();
                    break;

                case ActorAILogic_State.Lunging:
                    //Sprint the last little distance
                    Lunge();
                    break;

                    //no Attacking state on this logic... kinda interesting! Lunging replaces it.

                default:
                    Debug.LogError(ToString() + ": Unrecognized AI State!");
                    break;
            }

        }

        #region State Update Methods

        private void SearchForTargets()
        {
            if (EnemyExistsInAggroRadius())
            {
                ChooseNewTarget();
                ChangeState(ActorAILogic_State.Chasing);
            }
        }

        private void ChaseCurrentTarget()
        {
            //NavAgent.SetDestination(CurrentTarget.transform.position);

            if (CurrentTarget == null)
            {
                ChangeState(ActorAILogic_State.Idle);
            }
            else if (
                (CurrentTarget.transform.position - transform.position).sqrMagnitude <= Mathf.Pow(C_Preset.LungeOptions.LungePrepareDistance,2)
                && (NavAgent.path.corners.Length < 3) //straight shot
                )
            {
                ChangeState(ActorAILogic_State.ChargingAttack);
            }
            else if (Vector3.Angle(gameObject.transform.forward, (CurrentTarget.transform.position - gameObject.transform.position).normalized) <= Preset.Base.MaxFacingAngle / 2)
            {
                NavAgent.SetDestination(CurrentTarget.transform.position);
            }
            else
            {
                NavAgent.SetDestination(transform.position);
            }

        }

        private void WaitToLunge()
        {
            UnityEngine.AI.NavMeshPath NavPath = new UnityEngine.AI.NavMeshPath();

            if (CurrentTarget == null)
            {
                ChangeState(ActorAILogic_State.Idle);
            }

            //Target exceeds Lunge Loss distance
            else if ((CurrentTarget.transform.position - transform.position).sqrMagnitude >= Mathf.Pow(C_Preset.LungeOptions.LungeLosePrepareDistance, 2))
            {
                ChangeState(ActorAILogic_State.Chasing);
            }

            //Target is outside of Max Facing Angle
            else if (Vector3.Angle(gameObject.transform.forward, (CurrentTarget.transform.position - gameObject.transform.position).normalized) > Preset.Base.MaxFacingAngle / 2)
            {
                ChangeState(ActorAILogic_State.Chasing);
            }

            //Target is around some form of corner
            else if (NavAgent.CalculatePath(CurrentTarget.transform.position, NavPath) && !(NavAgent.path.corners.Length < 3))
            {
                ChangeState(ActorAILogic_State.Chasing);
            }

            //If we've waited long enough, change state to Lunging
            else if (Time.time - Info.LungeStartTime > C_Preset.LungeOptions.LungePause)
            {
                ChangeState(ActorAILogic_State.Lunging);
            }
        }

        private void Lunge()
        {
            if ((Time.time - Info.LungeStartTime) > C_Preset.LungeOptions.LungeTimeout)
            {
                ChangeState(ActorAILogic_State.Chasing);
            }
        }

        #endregion

        #endregion

        #region State Change

        protected override void StateBegin(ActorAILogic_State stateToStart)
        {
            base.StateBegin(stateToStart);

            switch (stateToStart)
            {
                case ActorAILogic_State.Idle:
                    Info.CanTurn = true;
                    break;

                case ActorAILogic_State.Chasing:
                    Info.CanTurn = true;
                    break;

                case ActorAILogic_State.ChargingAttack:
                    Info.CanTurn = true;
                    //NavAgent.SetDestination(transform.position + (transform.forward * Preset.Base.StopSlideDistance));
                    Info.LungeStartTime = Time.time;
                    if(Preset.Base.GrowDuration > 0 && Preset.Base.GrowCurve != null) StartCoroutine(I_IncreaseScale());
                    break;

                case ActorAILogic_State.Lunging:
                    //new attack
                    Info.CurrentAttacksInvoked = 0;
                    //disable turning
                    Info.CanTurn = false;
                    //NavAgent.speed = C_Preset.LungeSpeed;
                    //NavAgent.SetDestination(transform.position + (transform.forward * (GEAI_Preset.GEAI_Options.LungeDistance + GEAI_Preset.Base.StopSlideDistance)));
                    break;

                default:
                    Debug.LogError("AP2_GenericEnemyAI: Unrecognized AI State!");
                    break;
            }
        }

        protected override void StateEnd(ActorAILogic_State stateToEnd)
        {
            base.StateEnd(stateToEnd);

            switch(stateToEnd)
            {
                case ActorAILogic_State.Idle:
                    break;

                case ActorAILogic_State.Chasing:
                    break;

                case ActorAILogic_State.ChargingAttack:
                    break;

                case ActorAILogic_State.Lunging:
                    //NavAgent.SetDestination(transform.position);
                    NavAgent.speed = Preset.Base.MovementSpeed;
                    break;

                //no Attacking state on this logic... kinda interesting! Lunging replaces it.

                default:
                    Debug.LogError(ToString() + ": Unrecognized AI State!");
                    break;
            }
        }

        #endregion
    }

}