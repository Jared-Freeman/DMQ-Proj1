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

            ChangeState(ActorAILogic_State.Idle);
            StartCoroutine(UpdateAI());
        }

        #endregion


        /// <summary>
        /// Allows us to perform stuff on end of ActorAILogic_State. 
        /// This method is fired PRIOR to initializing the NEXT ActorAILogic_State during a ActorAILogic_State change.
        /// </summary>
        /// <param name="StateToEnd"></param>
        private void EndState(ActorAILogic_State StateToEnd)
        {
            switch (StateToEnd)
            {
                case ActorAILogic_State.Idle:
                    break;

                case ActorAILogic_State.Chasing:
                    break;

                case ActorAILogic_State.ChargingAttack:
                    break;

                case ActorAILogic_State.Attacking:
                    break;

                default:
                    Debug.LogError(ToString() + ": Unrecognized AI ActorAILogic_State!");
                    break;
            }
        }

        /// <summary>
        /// Fires when the Logic moves into a new ActorAILogic_State. Useful for initializing a ActorAILogic_State's variables or reporting changes to other objects
        /// </summary>
        /// <param name="Next_State"></param>
        protected override void ChangeState(ActorAILogic_State nextState)
        {
            base.ChangeState(nextState);

            //if (FLAG_Debug) Debug.Log("ActorAILogic_State CHANGE: " + CurrentState.ToString());

            switch (CurrentState)
            {
                case ActorAILogic_State.Idle:
                    Info.CanTurn = true;
                    break;

                case ActorAILogic_State.Chasing:
                    Info.CanTurn = true;
                    break;

                case ActorAILogic_State.ChargingAttack:
                    Info.CanTurn = true;
                    NavAgent.SetDestination(transform.position + (transform.forward * Preset.Base.StopSlideDistance));
                    Info.LungeStartTime = Time.time;
                    StartCoroutine(I_IncreaseScale());
                    break;

                    //This ActorAILogic_State is instantaneous, so we just do its thing and change ActorAILogic_State again.
                case ActorAILogic_State.Attacking:

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

                    ChangeState(ActorAILogic_State.Chasing);

                    break;

                default:
                    Debug.LogError("AP2_GenericEnemyAI: Unrecognized AI ActorAILogic_State!");
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
                    case ActorAILogic_State.Idle:
                        Info.TurnTarget = transform; //Probably not an amazing idea
                        SearchForTargets();
                        break;

                    case ActorAILogic_State.Chasing:
                        if (CurrentTarget != null)
                        {
                            Info.TurnTarget = CurrentTarget.transform;
                        }
                        ChaseCurrentTarget();
                        break;

                    case ActorAILogic_State.ChargingAttack:
                        PrepareAttack();
                        break;

                    case ActorAILogic_State.Attacking:
                        //Attack the target
                        break;

                    default:
                        Debug.LogError("AP2_GenericEnemyAI: Unrecognized AI ActorAILogic_State!");
                        break;
                }

                if (FLAG_Debug)
                {
                    Debug.DrawRay(NavAgent.destination, Vector3.up * 7f, Color.red, _RoutineSleepDuration);
                    //Debug.DrawRay(transform.position + new Vector3(0, 1.5f, 0), NavAgent.desiredVelocity * 2f, Color.black, _RoutineSleepDuration);
                }

                yield return new WaitForSeconds(_RoutineSleepDuration);
            }
        }

        private void PrepareAttack()
        {
            if (CurrentTarget == null)
            {
                ChangeState(ActorAILogic_State.Idle);
            }

            //Velocity Exceeds tolerance (if Preset flag is enabled)
            else if (!Preset.Base.AttackWhileMoving && RB.velocity.sqrMagnitude > Math.Pow(Preset.Base.StationaryVelocityThreshold, 2))
            {
                ChangeState(ActorAILogic_State.Chasing);
            }

            //Target exceeds Attack Loss distance
            else if ((CurrentTarget.transform.position - transform.position).sqrMagnitude >= Mathf.Pow(S_Preset.Shambler_Options.AttackLoseDistance, 2) )
            {
                ChangeState(ActorAILogic_State.Chasing);
            }

            //Target is around some form of corner
            else if (!(NavAgent.path.corners.Length < 3))
            {
                ChangeState(ActorAILogic_State.Chasing);
            }

            else if(!IsFacingTarget)
            {
                ChangeState(ActorAILogic_State.Chasing);
            }

            //If we've waited long enough, change ActorAILogic_State to Attacking. Ability must also be castable. Also must be facing target
            else if (Time.time - Info.LungeStartTime > S_Preset.Shambler_Options.AttackPause && Ability.CanCastAbility)
            {
                ChangeState(ActorAILogic_State.Attacking);
            }
        }

        private void ChaseCurrentTarget()
        {
            if (CurrentTarget == null)
            {
                ChangeState(ActorAILogic_State.Idle);
            }
            else if (
                Info.DistanceToCurrentTargetMagnitude_AtLastPoll <= S_Preset.Shambler_Options.AttackPrepareDistance
                //&& (NavAgent.path.corners.Length < 3) //straight shot. Still relevant to help enforce LOS on this shambler
                )
            {
                Vector3 dir2D = transform.position - CurrentTarget.transform.position;
                dir2D.y = 0;

                //here we do a raycast to see if anything occludes LOS. Slightly looser than Navagent corners method from earlier in dev't.
                RaycastHit hit;
                if (Physics.Raycast(
                    transform.position + new Vector3(0, .25f, 0) //TODO: Base this on some hitbox height -- we're currently using a magic number...
                    , -dir2D
                    , out hit
                    , S_Preset.Shambler_Options.AttackLoseDistance))
                {
                    //if (hit.collider.gameObject == gameObject) Debug.LogError("Need filtering!!! SELF!!!");
                    //else Debug.LogError("HIT: " + hit.collider.gameObject.name);

                    if (hit.collider.gameObject == CurrentTarget)
                    {
                        ChangeState(ActorAILogic_State.ChargingAttack);
                        return;
                    }
                }
            }
            //else
            //{
            //    Debug.LogWarning(Info.DistanceToCurrentTargetMagnitude_AtLastPoll);
            //}

            if (Vector3.Angle(gameObject.transform.forward, (CurrentTarget.transform.position - gameObject.transform.position).normalized) <= Preset.Base.MaxFacingAngle / 2)
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
                ChangeState(ActorAILogic_State.Chasing);
            }
        }


        private IEnumerator I_IncreaseScale()
        {
            var DefaultScale = transform.localScale;

            float StartTime = Time.time;
            while (
                CurrentState == ActorAILogic_State.ChargingAttack
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