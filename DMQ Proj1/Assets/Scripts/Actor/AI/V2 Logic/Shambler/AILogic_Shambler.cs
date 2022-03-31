using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AbilitySystem;
using EffectTree;
using System;

namespace ActorSystem.AI 
{

    /// <summary>
    /// A Shambler logic will simply approach an enemy and try to attack them using their attached Ability when within range.
    /// </summary>
    public class AILogic_Shambler : ActorAI_Logic
    {
        #region Members

        public AS_Ability_Instance_Base Ability { get; protected set; }
        public State CurrentState { get; protected set; }
        protected Rigidbody RB { get; set; }

        #region Properties

        public override ActorAI_Logic_PresetBase Preset 
        { 
            get => base.Preset as AILogic_ShamblerPreset; 
            set => base.Preset = value as AILogic_ShamblerPreset; 
        }
        public AILogic_ShamblerPreset S_Preset { get => Preset as AILogic_ShamblerPreset; set => Preset = value as AILogic_ShamblerPreset; }

        #endregion

        #region Helpers

        public enum State { Idle, Chasing, PrepAttack, Attacking }

        #endregion

        #endregion


        #region Initialization

        protected override void Awake()
        {
            base.Awake();

            Info.CanTurn = true; //redundant but whatever
            NavAgent.speed = Preset.Base.MovementSpeed;

            Ability = S_Preset.Shambler_Options.AttackAbility.GetInstance(gameObject);
            if (!Utils.Testing.ReferenceIsValid(Ability)) Destroy(this);

            RB = GetComponent<Rigidbody>();
            if (!Utils.Testing.ReferenceIsValid(RB)) Destroy(this);
        }

        protected override void Start()
        {
            base.Start();

            ChangeState(State.Idle);
            StartCoroutine(UpdateAI());
        }

        #endregion


        /// <summary>
        /// Allows us to perform stuff on end of state. 
        /// This method is fired PRIOR to initializing the NEXT state during a state change.
        /// </summary>
        /// <param name="StateToEnd"></param>
        private void EndState(State StateToEnd)
        {
            switch (StateToEnd)
            {
                case State.Idle:
                    break;

                case State.Chasing:
                    break;

                case State.PrepAttack:
                    break;

                case State.Attacking:
                    break;

                default:
                    Debug.LogError(ToString() + ": Unrecognized AI State!");
                    break;
            }
        }

        /// <summary>
        /// Fires when the Logic moves into a new State. Useful for initializing a state's variables or reporting changes to other objects
        /// </summary>
        /// <param name="Next_State"></param>
        private void ChangeState(State Next_State)
        {
            EndState(CurrentState);
            CurrentState = Next_State;

            if (FLAG_Debug) Debug.Log("STATE CHANGE: " + CurrentState.ToString());

            switch (CurrentState)
            {
                case State.Idle:
                    Info.CanTurn = true;
                    break;

                case State.Chasing:
                    Info.CanTurn = true;
                    break;

                case State.PrepAttack:
                    Info.CanTurn = true;
                    NavAgent.SetDestination(transform.position + (transform.forward * Preset.Base.StopSlideDistance));
                    Info.LungeStartTime = Time.time;
                    StartCoroutine(I_IncreaseScale());
                    break;

                    //This state is instantaneous, so we just do its thing and change state again.
                case State.Attacking:

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

                    Ability?.ExecuteAbility(ref ec);

                    ChangeState(State.Chasing);

                    break;

                default:
                    Debug.LogError("AP2_GenericEnemyAI: Unrecognized AI State!");
                    break;
            }

            return;

        }


        private IEnumerator UpdateAI()
        {
            while (true)
            {
                switch (CurrentState)
                {
                    case State.Idle:
                        Info.TurnTarget = transform; //Probably not an amazing idea
                        SearchForTargets();
                        break;

                    case State.Chasing:
                        if (CurrentTarget != null)
                        {
                            Info.TurnTarget = CurrentTarget.transform;
                        }
                        ChaseCurrentTarget();
                        break;

                    case State.PrepAttack:
                        PrepareAttack();
                        break;

                    case State.Attacking:
                        //Attack the target
                        break;

                    default:
                        Debug.LogError("AP2_GenericEnemyAI: Unrecognized AI State!");
                        break;
                }

                if (FLAG_Debug)
                {
                    Debug.DrawRay(NavAgent.destination, Vector3.up * 7f, Color.red, _RoutineSleepDuration);
                    Debug.DrawRay(transform.position + new Vector3(0, 1.5f, 0), NavAgent.desiredVelocity * 2f, Color.black, _RoutineSleepDuration);
                }

                yield return new WaitForSeconds(_RoutineSleepDuration);
            }
        }

        private void PrepareAttack()
        {
            if (CurrentTarget == null)
            {
                ChangeState(State.Idle);
            }

            //Velocity Exceeds tolerance (if Preset flag is enabled)
            else if (!Preset.Base.AttackWhileMoving && RB.velocity.sqrMagnitude > Math.Pow(Preset.Base.StationaryVelocityThreshold, 2))
            {
                ChangeState(State.Chasing);
            }

            //Target exceeds Attack Loss distance
            else if ((CurrentTarget.transform.position - transform.position).sqrMagnitude >= Mathf.Pow(S_Preset.Shambler_Options.AttackLoseDistance, 2) )
            {
                ChangeState(State.Chasing);
            }

            //Target is around some form of corner
            else if (!(NavAgent.path.corners.Length < 3))
            {
                ChangeState(State.Chasing);
            }

            else if(!IsFacingTarget)
            {
                ChangeState(State.Chasing);
            }

            //If we've waited long enough, change state to Attacking. Ability must also be castable. Also must be facing target
            else if (Time.time - Info.LungeStartTime > S_Preset.Shambler_Options.AttackPause && Ability.CanCastAbility)
            {
                ChangeState(State.Attacking);
            }
        }

        private void ChaseCurrentTarget()
        {
            if (CurrentTarget == null)
            {
                ChangeState(State.Idle);
            }
            else if (
                (CurrentTarget.transform.position - transform.position).sqrMagnitude <= Mathf.Pow(S_Preset.Shambler_Options.AttackPrepareDistance, 2)
                && (NavAgent.path.corners.Length < 3) //straight shot. Still relevant to help enforce LOS on this shambler
                )
            {
                ChangeState(State.PrepAttack);
            }
            else if (Vector3.Angle(gameObject.transform.forward, (CurrentTarget.transform.position - gameObject.transform.position).normalized) <= Preset.Base.MaxFacingAngle / 2)
            {
                NavAgent.SetDestination(CurrentTargetPosition);
                if(NavAgent.path.corners.Length > 3) NavAgent.SetDestination(CurrentTarget.transform.position);
            }
            else
            {
                NavAgent.SetDestination(transform.position);
            }
        }

        private void SearchForTargets()
        {
            if (EnemyExistsInAggroRadius())
            {
                ChooseNewTarget();
                ChangeState(State.Chasing);
            }
        }


        private IEnumerator I_IncreaseScale()
        {
            var DefaultScale = transform.localScale;

            float StartTime = Time.time;
            while (
                CurrentState == State.PrepAttack 
                && Time.time - StartTime < S_Preset.Shambler_Options.AttackPause
                && IsFacingTarget
                && (Preset.Base.AttackWhileMoving || (RB.velocity.sqrMagnitude < Math.Pow(Preset.Base.StationaryVelocityThreshold, 2)))
                )
            {
                //if (FLAG_Debug) Debug.Log("THRESH: " + (Preset.Base.AttackWhileMoving || (RB.velocity.sqrMagnitude < Math.Pow(Preset.Base.StationaryVelocityThreshold, 2))));

                transform.localScale = DefaultScale * S_Preset.Shambler_Options.GrowCurve.Evaluate((Time.time - StartTime) / S_Preset.Shambler_Options.AttackPause);
                yield return null;
            }

            transform.localScale = DefaultScale;
        }

    }

}