using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AbilitySystem;
using EffectTree;

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

        public ChargerStateInfo C_StateInfo { get; protected set; } = new ChargerStateInfo();

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

        public class ChargerStateInfo
        {
            public float ChargeAttackAnimationSpeedMultiplier { get; set; } = 1f;
        }


        #endregion



        #region Initialization

        protected override void Awake()
        {
            base.Awake();

            _AbilityInstance_OnCollideEnter = C_Preset.Ability_OnChargeCollision.GetInstance(gameObject);
            _AbilityInstance_OnLungeBegin = C_Preset.Ability_OnLungeBegin.GetInstance(gameObject);
            //no test here because there may be use case for leaving _AbilityInstance's null

            C_StateInfo.ChargeAttackAnimationSpeedMultiplier = C_Preset.LungeOptions.LungeAnimationClipLength / C_Preset.LungeOptions.LungePause;
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
            // TODO: write some OPTIONAL logic here to see if there are obstacles in the way to prevent charging behavior
            else if (
                _AbilityInstance_OnLungeBegin.CanCastAbility
                && Info.DistanceToCurrentTargetMagnitude_AtLastPoll <= C_Preset.LungeOptions.LungePrepareDistance
                //&& (CurrentTarget.transform.position - transform.position).sqrMagnitude <= Mathf.Pow(C_Preset.LungeOptions.LungePrepareDistance,2)
                //&& (NavAgent.path.corners.Length < 3) //straight shot
                )
            {
                ChangeState(ActorAILogic_State.ChargingAttack);
            }
            else if (Vector3.Angle(gameObject.transform.forward, -Info.DistanceToCurrentTarget_AtLastPoll.normalized) <= Preset.Base.MaxFacingAngle / 2)
            {
                Debug.LogWarning("facing. No need to turn. Target: " + CurrentTarget.ToString());
                NavAgent.SetDestination(CurrentTarget.transform.position);
            }
            else
            {
                Debug.LogWarning("not facing. Need to turn");
                NavAgent.SetDestination(transform.position);
            }

        }

        private void WaitToLunge()
        {
            if (CurrentTarget == null)
            {
                ChangeState(ActorAILogic_State.Idle);
            }

            //Target exceeds Lunge Loss distance
            else if (Preset.Base.CanCancelAttackEarly)
            {
                if(Info.DistanceToCurrentTargetMagnitude_AtLastPoll >= C_Preset.LungeOptions.LungeLosePrepareDistance)
                {
                    ChangeState(ActorAILogic_State.Chasing);
                }
                //Target is outside of Max Facing Angle
                else if (Vector3.Angle(gameObject.transform.forward, Info.DistanceToCurrentTarget_AtLastPoll.normalized) > Preset.Base.MaxFacingAngle / 2)
                {
                    ChangeState(ActorAILogic_State.Chasing);
                }
            }

            //If we've waited long enough, change state to Lunging
            else if (Mathf.Abs(Time.time - Info.LungeStartTime) > C_Preset.LungeOptions.LungePause)
            {
                ChangeState(ActorAILogic_State.Lunging);
            }
        }

        private void Lunge()
        {
            //exit conditions
            if (Mathf.Abs(Time.time - Info.LungeStartTime) > C_Preset.LungeOptions.LungeTimeout)
            {
                ChangeState(ActorAILogic_State.Chasing);
            }
            else
            {
                NavAgent.SetDestination(transform.position); //continue sync of navagent destination polling. may be removeable...
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
                    CanMove = false;
                    NavAgent.SetDestination(transform.position);
                    Info.LungeStartTime = Time.time;
                    if(Preset.Base.UseGrowCurve && Preset.Base.GrowDuration > 0 && Preset.Base.GrowCurve != null) StartCoroutine(I_IncreaseScale());

                    Invoke_OnAttackChargeBegin(new EventArgs.ActorAI_Logic_EventArgs(AttachedActor, 1, C_StateInfo.ChargeAttackAnimationSpeedMultiplier));
                    break;

                case ActorAILogic_State.Lunging:
                    Info.LungeStartTime = Time.time;

                    Invoke_OnAttackStart(new EventArgs.ActorAI_Logic_EventArgs(AttachedActor, 2));

                    NavAgent.SetDestination(transform.position);

                    //new attack
                    Info.CurrentAttacksInvoked = 0;
                    //disable turning and AI mover
                    Info.CanTurn = false;
                    CanMove = false;

                    if(CurrentTarget != null)
                    {
                        Utils.AttackContext ac = new Utils.AttackContext()
                        {
                            _InitialDirection = transform.forward,
                            _InitialGameObject = gameObject,
                            _InitialPosition = transform.position,
                            _Owner = AttachedActor,
                            _TargetDirection = CurrentTarget.gameObject.transform.position - gameObject.transform.position,
                            _TargetGameObject = CurrentTarget.gameObject,
                            _TargetPosition = CurrentTarget.gameObject.transform.position,
                            _Team = AttachedActor._Team
                        };

                        EffectContext ec = new EffectContext(ac);

                        _AbilityInstance_OnLungeBegin.ExecuteAbility(ref ec);
                    }

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
                    CanMove = true;
                    if(CurrentState != ActorAILogic_State.Lunging)
                    {
                        Invoke_OnAttackChargeCancel(new EventArgs.ActorAI_Logic_EventArgs(AttachedActor, 1));
                    }
                    break;

                case ActorAILogic_State.Lunging:
                    CanMove = true;
                    CanTurn = true;

                    Invoke_OnAttackEnd(new EventArgs.ActorAI_Logic_EventArgs(AttachedActor, 2));

                    //NavAgent.SetDestination(transform.position);
                    //NavAgent.speed = Preset.Base.MovementSpeed;

                    CanMove = true;
                    break;

                //no Attacking state on this logic... kinda interesting! Lunging replaces it.

                default:
                    Debug.LogError(ToString() + ": Unrecognized AI State!");
                    break;
            }
        }

        #endregion

        #region Collision Event(s)


        private void OnCollisionEnter(Collision collision)
        {
            if (CurrentState == ActorAILogic_State.Lunging)
            {
                //TODO: Check this logic and refactor. For now the branch is just turned on.
                if (true ||
                    CurrentTarget != null
                    && Info.CurrentAttacksInvoked < 1
                    && collision.gameObject == CurrentTarget
                    //&& (CurrentTarget.transform.position - transform.position).sqrMagnitude < GEAI_Preset.Base.AttackRange * GEAI_Preset.Base.AttackRange //can probably remove w new syst
                    )
                {
                    if (FLAG_Debug) Debug.Log(ToString() + ": ATTACKING " + CurrentTarget.name);

                    Info.CurrentAttacksInvoked++;

                    //invoke ability
                    if (_AbilityInstance_OnCollideEnter != null && _AbilityInstance_OnCollideEnter.CanCastAbility)
                    {
                        EffectContext.EffectContextInfo cData = EffectContext.CreateContextDataFromCollision(collision);

                        Utils.AttackContext ac = new Utils.AttackContext()
                        {
                            _InitialDirection = transform.forward,
                            _InitialGameObject = gameObject,
                            _InitialPosition = transform.position,
                            _Owner = AttachedActor,
                            _TargetDirection = collision.gameObject.transform.position - gameObject.transform.position,
                            _TargetGameObject = collision.gameObject,
                            _TargetPosition = collision.gameObject.transform.position,
                            _Team = AttachedActor._Team
                        };

                        EffectContext ec = new EffectContext(ac, cData);

                        _AbilityInstance_OnCollideEnter.ExecuteAbility(ref ec);
                    }
                }

            }
        }

        #endregion
    }

}