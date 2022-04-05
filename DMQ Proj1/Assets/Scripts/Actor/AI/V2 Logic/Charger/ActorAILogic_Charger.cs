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

        protected AS_Ability_Instance_Base _AbilityInstance_BasicAttack;
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
            public float ChargeBasicAttackAnimationSpeedMultiplier { get; set; } = 1f;

            public float BasicAttackStartTime = 0f;
        }


        #endregion



        #region Initialization

        protected override void Awake()
        {
            base.Awake();

            _AbilityInstance_BasicAttack = C_Preset.Ability_BasicAttack.GetInstance(gameObject);
            _AbilityInstance_OnCollideEnter = C_Preset.Ability_OnChargeCollision.GetInstance(gameObject);
            _AbilityInstance_OnLungeBegin = C_Preset.Ability_OnLungeBegin.GetInstance(gameObject);
            //no test here because there may be use case for leaving _AbilityInstance's null

            C_StateInfo.ChargeAttackAnimationSpeedMultiplier = C_Preset.LungeOptions.LungeAnimationClipLength / C_Preset.LungeOptions.LungePause;
            C_StateInfo.ChargeBasicAttackAnimationSpeedMultiplier = C_Preset.BasicAttackOptions.AnimationClipLength / C_Preset.BasicAttackOptions.AttackPause;
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

                case ActorAILogic_State.ChargingAttack2:
                    PrepareBasicAttack();
                    break;
                case ActorAILogic_State.Attacking2:
                    break;

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
                && Info.DistanceToCurrentTargetMagnitude_AtLastPoll > C_Preset.LungeOptions.LungeTooCloseDistance
                //&& (CurrentTarget.transform.position - transform.position).sqrMagnitude <= Mathf.Pow(C_Preset.LungeOptions.LungePrepareDistance,2)
                //&& (NavAgent.path.corners.Length < 3) //straight shot
                )
            {
                ChangeState(ActorAILogic_State.ChargingAttack);
            }




            else if 
                (
                _AbilityInstance_BasicAttack.CanCastAbility
                && Info.DistanceToCurrentTargetMagnitude_AtLastPoll <= C_Preset.BasicAttackOptions.PrepareDistance
                && Info.AttachedRigidbody.velocity.sqrMagnitude <= Mathf.Pow(Preset.Base.StationaryVelocityThreshold,2)
                //&& (NavAgent.path.corners.Length < 3) //straight shot. Still relevant to help enforce LOS on this shambler
                )
            {
                Vector3 dir2D = transform.position - CurrentTarget.transform.position;
                dir2D.y = 0;

                if(C_Preset.BasicAttackOptions.NeedLineOfSightToAttack)
                {
                    //here we do a raycast to see if anything occludes LOS. Slightly looser than Navagent corners method from earlier in dev't.
                    RaycastHit hit;
                    if (Physics.Raycast(
                        transform.position + new Vector3(0, .25f, 0) //TODO: Base this on some hitbox height -- we're currently using a magic number...
                        , -dir2D
                        , out hit
                        , C_Preset.BasicAttackOptions.LosePrepareDistance))
                    {
                        //if (hit.collider.gameObject == gameObject) Debug.LogError("Need filtering!!! SELF!!!");
                        //else Debug.LogError("HIT: " + hit.collider.gameObject.name);

                        if (hit.collider.gameObject == CurrentTarget)
                        {
                            ChangeState(ActorAILogic_State.ChargingAttack2);
                            return;
                        }
                    }
                }
                else
                {
                    ChangeState(ActorAILogic_State.ChargingAttack2);
                    return;
                }
            }


            else if (
                //we're not in attack range and dont need to turn
                Info.DistanceToCurrentTargetMagnitude_AtLastPoll > C_Preset.BasicAttackOptions.PrepareDistance
                && Vector3.Angle(gameObject.transform.forward, (CurrentTarget.transform.position - gameObject.transform.position).normalized) <= Preset.Base.MaxFacingAngle / 2
                )
            {
                NavAgent.SetDestination(CurrentTargetPosition);
                //if (NavAgent.path.corners.Length > 3) NavAgent.SetDestination(CurrentTarget.transform.position);
            }
            else
            {
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
                ChangeState(ActorAILogic_State.Idle);
            }
            else
            {
                //DesiredVelocity = Vector3.zero;
                //NavAgent.SetDestination(transform.position); //continue sync of navagent destination polling. may be removeable...
            }
        }


        private void PrepareBasicAttack()
        {
            //Here we HAVE TO WAIT the full duration of charging regardless of any other circumstance.
            if (!Preset.Base.CanCancelAttackEarly && Time.time - C_StateInfo.BasicAttackStartTime < C_Preset.BasicAttackOptions.AttackPause)
            {
                return;
            }

            else if (CurrentTarget == null)
            {
                ChangeState(ActorAILogic_State.Idle);
            }

            //Velocity Exceeds tolerance (if Preset flag is enabled)
            else if (!Preset.Base.AttackWhileMoving && Info.AttachedRigidbody.velocity.sqrMagnitude > Mathf.Pow(Preset.Base.StationaryVelocityThreshold, 2))
            {
                ChangeState(ActorAILogic_State.Chasing);
            }

            //Target exceeds Attack Loss distance
            else if ((CurrentTarget.transform.position - transform.position).sqrMagnitude >= Mathf.Pow(C_Preset.BasicAttackOptions.LosePrepareDistance, 2))
            {
                ChangeState(ActorAILogic_State.Chasing);
            }

            //Target is around some form of corner
            else if (!(NavAgent.path.corners.Length < 3))
            {
                ChangeState(ActorAILogic_State.Chasing);
            }

            else if (!IsFacingTarget)
            {
                ChangeState(ActorAILogic_State.Chasing);
            }

            //If we've waited long enough, change ActorAILogic_State to Attacking. Ability must also be castable. Also must be facing target
            else if (Time.time - C_StateInfo.BasicAttackStartTime > C_Preset.BasicAttackOptions.AttackPause && _AbilityInstance_BasicAttack.CanCastAbility)
            {
                ChangeState(ActorAILogic_State.Attacking2);
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
                    CanMove = true;
                    CanTurn = true;
                    break;

                case ActorAILogic_State.Chasing:
                    Info.CanTurn = true;
                    Info.CanMove = true;
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

                        EffectContext effectC = new EffectContext(ac);

                        _AbilityInstance_OnLungeBegin.ExecuteAbility(ref effectC);
                    }

                    break;


                case ActorAILogic_State.ChargingAttack2:
                    C_StateInfo.BasicAttackStartTime = Time.time;

                    Invoke_OnAttackChargeBegin(new ActorSystem.AI.EventArgs.ActorAI_Logic_EventArgs(AttachedActor, 3, C_StateInfo.ChargeBasicAttackAnimationSpeedMultiplier));
                    break;

                case ActorAILogic_State.Attacking2:

                    Utils.AttackContext a = new Utils.AttackContext();

                    a._InitialDirection = gameObject.transform.forward;
                    a._InitialGameObject = gameObject;
                    a._InitialPosition = transform.position;

                    a._Owner = AttachedActor;
                    a._Team = AttachedActor._Team;

                    a._TargetDirection = (transform.position - CurrentTarget.transform.position).normalized;
                    a._TargetPosition = CurrentTarget.transform.position;
                    a._TargetGameObject = CurrentTarget;

                    EffectContext ec = new EffectContext(a);

                    _AbilityInstance_BasicAttack?.ExecuteAbility(ref ec);


                    ChangeState(ActorAILogic_State.Chasing);

                    //Dispatch event to AI animator proxy
                    //OnAbilityCast?.Invoke(this, new AILogic_ShamblerEventArgs(AttachedActor, 1)); //Using index 1 for now

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
                    break;


                case ActorAILogic_State.ChargingAttack2:
                    if (CurrentState != ActorAILogic_State.Attacking2)
                    {
                        Invoke_OnAttackChargeCancel(new ActorSystem.AI.EventArgs.ActorAI_Logic_EventArgs(AttachedActor, 3));

                        //old
                        //OnAttackChargeCancel?.Invoke(this, new AILogic_ShamblerEventArgs(AttachedActor, 1));
                    }
                    break;

                case ActorAILogic_State.Attacking2:
                    
                    Invoke_OnAttackEnd(new EventArgs.ActorAI_Logic_EventArgs(AttachedActor, 3));
                    break;


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