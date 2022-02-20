using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using AbilitySystem;
using EffectTree;

namespace ActorSystem.AI
{
    /// <summary>
    /// This AI Logic is intended to simply walk to a point near the player, lunge, and perform an attack upon a collision DURING the lunge
    /// </summary>
    public class AP2_GenericEnemyAI : ActorAI_Logic
    {
        public static float s_remainingDistanceTolerance = .2f;

        //public Utils.CooldownTracker AttackCooldown;
        //public AP2_ActorAction_AttackTarget AttackAction; //TODO: Change to ability

        protected AS_Ability_Instance_Base _Ability;


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
                return base.Preset as AP2_GenericEnemyAI_LogicPreset;
            }
            set
            {
                base.Preset = value as AP2_GenericEnemyAI_LogicPreset;
            }
        }
        /// <summary>
        /// convenience property that wraps Preset
        /// </summary>
        public AP2_GenericEnemyAI_LogicPreset GEAI_Preset 
        { 
            get { return Preset as AP2_GenericEnemyAI_LogicPreset; } 
            protected set { Preset = value; } 
        }

        //state vars
        public State CurrentState { get; protected set; }

        #region Helpers


        public enum State { Idle, Chasing, PrepToLunge, Lunging, Attacking }
        #endregion


        #region Init
        protected override void Awake()
        {
            base.Awake();

            //AttackCooldown.InitializeCooldown();
            NavAgent.speed = GEAI_Preset.Base.MovementSpeed;

            _Ability = GEAI_Preset.GEAI_Options.LungeCollisionAbility.GetInstance(gameObject);
            if (_Ability == null) Debug.LogError("No ability found");
        }

        protected override void Start()
        {
            base.Start();

            ChangeState(State.Idle);
            StartCoroutine(UpdateAI());
        }
        #endregion


        private void OnDrawGizmos()
        {
            if(GEAI_Preset.Base.DrawDebugGizmos)
            {
                Gizmos.DrawWireSphere(transform.position, GEAI_Preset.Base.AggroRadius);

                Gizmos.DrawWireSphere(transform.position, GEAI_Preset.GEAI_Options.LungePrepareDistance);

                if (CurrentTarget != null) Gizmos.DrawRay(CurrentTarget.transform.position, Vector3.up * 4f);
            }
        }


        private IEnumerator UpdateAI()
        {
            while (true)
            {
                switch (CurrentState)
                {
                    case State.Idle:
                        SearchForTargets();
                        break;

                    case State.Chasing:
                        ChaseCurrentTarget();
                        break;

                    case State.PrepToLunge:
                        WaitToLunge();
                        break;

                    case State.Lunging:
                        //Sprint the last little distance
                        Lunge();
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
                    Debug.DrawRay(transform.position + new Vector3(0,1.5f,0), NavAgent.desiredVelocity * 2f, Color.black, _RoutineSleepDuration);
                }

                yield return new WaitForSeconds(_RoutineSleepDuration);
            }
        }

        private void ChangeState(State CurState)
        {
            CurrentState = CurState;
            if (FLAG_Debug) Debug.Log("STATE CHANGE: " + CurState.ToString());

            switch (CurState)
            {
                case State.Idle:
                    Info.CanTurn = true;
                    break;

                case State.Chasing:
                    Info.CanTurn = true;
                    break;

                case State.PrepToLunge:
                    Info.CanTurn = true;
                    NavAgent.SetDestination(transform.position + (transform.forward * GEAI_Preset.Base.StopSlideDistance));
                    Info.LungeStartTime = Time.time;
                    StartCoroutine(I_IncreaseScale());
                    break;

                case State.Lunging:
                    //new attack
                    Info.CurrentAttacksInvoked = 0;
                    //disable turning
                    Info.CanTurn = false;
                    NavAgent.speed = GEAI_Preset.GEAI_Options.LungeSpeed;
                    NavAgent.SetDestination(transform.position + (transform.forward * (GEAI_Preset.GEAI_Options.LungeDistance + GEAI_Preset.Base.StopSlideDistance)));
                    break;

                case State.Attacking:
                    break;

                default:
                    Debug.LogError("AP2_GenericEnemyAI: Unrecognized AI State!");
                    break;
            }

            return;

        }

        private void OnCollisionEnter(Collision collision)
        {
            if (CurrentState == State.Lunging)
            {
                if (
                    CurrentTarget != null
                    && Info.CurrentAttacksInvoked < 1
                    && collision.gameObject == CurrentTarget
                    //&& (CurrentTarget.transform.position - transform.position).sqrMagnitude < GEAI_Preset.Base.AttackRange * GEAI_Preset.Base.AttackRange //can probably remove w new syst
                    )
                {
                    if (FLAG_Debug) Debug.Log("ATTACKING " + CurrentTarget.name);
                    //AttackAction.AttackTarget(AttachedActor, CurrentTarget);
                    Info.CurrentAttacksInvoked++;

                    if(_Ability != null && _Ability.CanCastAbility)
                    {
                        Utils.AttackContext ac = new Utils.AttackContext()
                        {
                            _InitialDirection = collision.gameObject.transform.position - gameObject.transform.position,
                            _InitialGameObject = gameObject,
                            _InitialPosition = transform.position,
                            _Owner = AttachedActor,
                            _TargetDirection = collision.gameObject.transform.position - gameObject.transform.position,
                            _TargetGameObject = collision.gameObject,
                            _TargetPosition = collision.gameObject.transform.position,
                            _Team = AttachedActor._Team
                        };

                        EffectContext ec = new EffectContext(ac);

                        _Ability.ExecuteAbility(ref ec);
                    }

                    //Obsolete. (2-19-2022) ~Jared
                    //if(GEAI_Preset.GEAI_Options.Lunge_ImpactEffects != null)
                    //{
                    //    foreach(var IFX in GEAI_Preset.GEAI_Options.Lunge_ImpactEffects)
                    //    {
                    //        IFX.SpawnImpactEffect(null, collision.contacts[0].point, collision.contacts[0].normal);
                    //    }
                    //}
                }

            }
        }


        #region Utility Routines
        protected IEnumerator I_IncreaseScale()
        {
            var DefaultScale = transform.localScale;

            float StartTime = Time.time;
            while (CurrentState == State.PrepToLunge && Time.time - StartTime < GEAI_Preset.Base.GrowDuration )
            {
                transform.localScale = DefaultScale * GEAI_Preset.Base.GrowCurve.Evaluate((Time.time - StartTime) / GEAI_Preset.Base.GrowDuration);
                yield return null;
            }

            transform.localScale = DefaultScale;
        }

        #endregion

        #region State Update Methods
        private void SearchForTargets()
        {
            if(EnemyExistsInAggroRadius())
            {
                ChooseNewTarget();
                ChangeState(State.Chasing);
            }
        }

        private void ChaseCurrentTarget()
        {
            //NavAgent.SetDestination(CurrentTarget.transform.position);

            if(CurrentTarget == null)
            {
                ChangeState(State.Idle);
            }
            else if (
                (CurrentTarget.transform.position - transform.position).sqrMagnitude <= GEAI_Preset.GEAI_Options.LungePrepareDistance * GEAI_Preset.GEAI_Options.LungePrepareDistance
                && (NavAgent.path.corners.Length < 3) //straight shot
                )
            {
                ChangeState(State.PrepToLunge);
            }
            else if (Vector3.Angle(gameObject.transform.forward, (CurrentTarget.transform.position - gameObject.transform.position).normalized) <= GEAI_Preset.Base.MaxFacingAngle / 2)
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
                ChangeState(State.Idle);
            }

            //Target exceeds Lunge Loss distance
            else if ((CurrentTarget.transform.position - transform.position).sqrMagnitude >= GEAI_Preset.GEAI_Options.LungeLosePrepareDistance * GEAI_Preset.GEAI_Options.LungeLosePrepareDistance)
            {
                ChangeState(State.Chasing);
            }

            //Target is outside of Max Facing Angle
            else if (Vector3.Angle(gameObject.transform.forward, (CurrentTarget.transform.position - gameObject.transform.position).normalized) > GEAI_Preset.Base.MaxFacingAngle / 2)
            {
                ChangeState(State.Chasing);
            }

            //Target is around some form of corner
            else if(NavAgent.CalculatePath(CurrentTarget.transform.position, NavPath) && !(NavAgent.path.corners.Length < 3))
            {
                ChangeState(State.Chasing);
            }

            //If we've waited long enough, change state to Lunging
            else if(Time.time - Info.LungeStartTime > GEAI_Preset.GEAI_Options.LungePause)
            {
                ChangeState(State.Lunging);
            }
        }

        private void Lunge()
        {

            if (NavAgent.remainingDistance < s_remainingDistanceTolerance || (Time.time - Info.LungeStartTime) > GEAI_Preset.GEAI_Options.LungeTimeout) //magic number :(
            {
                NavAgent.SetDestination(transform.position);
                NavAgent.speed = GEAI_Preset.Base.MovementSpeed;
                ChangeState(State.Chasing);
            }
        }
        #endregion



        //I'm still a bit mad that this solution concept doesnt work and I (naively) blame Unity
        #region Dumpstered Methods

        //private void OldChangeState(State CurState)
        //{
        //    //apparently you can ONLY stop coroutines when a string is supplied to make them?? Thanks unity...
        //    foreach (Coroutine C in Info.ActiveRoutines)
        //    {
        //        if (C == null) Debug.LogError("Routine not found!!!");
        //        else StopCoroutine(C.ToString());
        //    }
        //    StopAllCoroutines();


        //    Info.ActiveRoutines.Clear();

        //    CurrentState = CurState;

        //    if (FLAG_Debug) Debug.Log("STATE CHANGE: " + CurState.ToString());

        //    switch (CurState)
        //    {
        //        case State.Idle:
        //            //Look for a bad guy to chase
        //            Info.ActiveRoutines.Add(StartCoroutine("I_SearchForTargets"));
        //            break;

        //        case State.Chasing:
        //            //Consider checking for a better target here at some point
        //            //Check for target death (subscribe to an event?)
        //            //Get in attacking range of current Target

        //            //turning needs to be interpolated per-frame. Added it to current routines stack
        //            Info.ActiveRoutines.Add(StartCoroutine("I_TurnInterpolate"));
        //            Info.ActiveRoutines.Add(StartCoroutine("I_ChaseCurrentTarget"));
        //            break;

        //        case State.PrepToLunge:
        //            //Brief pause before Lunging. Target may exit lunge range in this time. Gives target the chance to dodge or disengage

        //            //turning needs to be interpolated per-frame. Added it to current routines stack
        //            Info.ActiveRoutines.Add(StartCoroutine("I_TurnInterpolate"));
        //            Info.ActiveRoutines.Add(StartCoroutine("I_WaitToLunge"));
        //            break;

        //        case State.Lunging:
        //            //Sprint the last little distance
        //            Info.ActiveRoutines.Add(StartCoroutine("I_Lunge"));
        //            break;

        //        case State.Attacking:
        //            //Attack the target
        //            break;

        //        default:
        //            Debug.LogError("AP2_GenericEnemyAI: Unrecognized AI State!");
        //            break;
        //    }
        //}

        private IEnumerator I_SearchForTargets()
        {
            //Do nothing until enemy in radius
            while (!EnemyExistsInAggroRadius())
            {
                yield return new WaitForSeconds(_RoutineSleepDuration);
            }

            ChooseNewTarget();
            ChangeState(State.Chasing);
        }


        private IEnumerator I_ChaseCurrentTarget()
        {
            bool CanLunge = false;

            if ((CurrentTarget.transform.position - transform.position).sqrMagnitude <= GEAI_Preset.GEAI_Options.LungePrepareDistance * GEAI_Preset.GEAI_Options.LungePrepareDistance) CanLunge = true;

            while (CurrentTarget != null && !CanLunge)
            {
                if ((CurrentTarget.transform.position - transform.position).sqrMagnitude <= GEAI_Preset.GEAI_Options.LungePrepareDistance * GEAI_Preset.GEAI_Options.LungePrepareDistance)
                {
                    if (FLAG_Debug) Debug.DrawRay(gameObject.transform.position, gameObject.transform.up * 6f, Color.yellow, _RoutineSleepDuration);

                    NavAgent.SetDestination(transform.position);

                    CanLunge = true;


                    ////only turning
                    //NavAgent.updatePosition = false;
                    //NavAgent.SetDestination((CurrentTarget.transform.position - gameObject.transform.position).normalized);
                }
                else if (Vector3.Angle(gameObject.transform.forward, (CurrentTarget.transform.position - gameObject.transform.position).normalized) <= GEAI_Preset.Base.MaxFacingAngle / 2)
                {
                    NavAgent.SetDestination(CurrentTarget.transform.position);
                }
                else
                {
                    NavAgent.SetDestination(transform.position);
                }


                if (FLAG_Debug)
                {
                    Debug.DrawRay(gameObject.transform.position, gameObject.transform.forward * 3f, Color.blue, _RoutineSleepDuration);
                    Debug.DrawRay(gameObject.transform.position, (CurrentTarget.transform.position - gameObject.transform.position).normalized * 3f, Color.red, _RoutineSleepDuration);
                    //Debug.Log("ANGLE: " + Vector3.Angle(gameObject.transform.forward, (CurrentTarget.transform.position - gameObject.transform.position).normalized));
                }

                yield return new WaitForSeconds(_RoutineSleepDuration);
            }

            if (CanLunge) ChangeState(State.PrepToLunge);
            else ChangeState(State.Idle);
        }

        private IEnumerator I_WaitToLunge()
        {
            float StartTime = Time.time;
            float CurTime = Time.time;

            while (
                CurrentTarget != null
                && Mathf.Abs(CurTime - StartTime) < GEAI_Preset.GEAI_Options.LungePause
                && (CurrentTarget.transform.position - transform.position).sqrMagnitude <= GEAI_Preset.GEAI_Options.LungeLosePrepareDistance * GEAI_Preset.GEAI_Options.LungeLosePrepareDistance
                && (Vector3.Angle(gameObject.transform.forward, (CurrentTarget.transform.position - gameObject.transform.position).normalized) <= GEAI_Preset.Base.MaxFacingAngle / 2)
                )
            {
                CurTime = Time.time;

                //if (FLAG_Debug)
                //{
                //    Debug.Log("Duration Complete: " + (Mathf.Abs(CurTime - StartTime) < GEAI_Preset.Base.LungePause).ToString());
                //    Debug.Log("Angle Complete: " + ((Vector3.Angle(gameObject.transform.forward, (CurrentTarget.transform.position - gameObject.transform.position).normalized) <= GEAI_Preset.Base.MaxFacingAngle / 2)).ToString());

                //    Debug.Log("Distance Complete: " + ((CurrentTarget.transform.position - transform.position).sqrMagnitude <= GEAI_Preset.Base.LungeLosePrepareDistance * GEAI_Preset.Base.LungeLosePrepareDistance).ToString());
                //}

                yield return new WaitForSeconds(_RoutineSleepDuration);
            }

            if (CurrentTarget == null)
                ChangeState(State.Idle);

            else if ((CurrentTarget.transform.position - transform.position).sqrMagnitude > GEAI_Preset.GEAI_Options.LungeLosePrepareDistance * GEAI_Preset.GEAI_Options.LungeLosePrepareDistance)
                ChangeState(State.Chasing);

            else
                ChangeState(State.Lunging);

        }
        private IEnumerator I_Lunge()
        {
            NavAgent.speed = GEAI_Preset.GEAI_Options.LungeSpeed;
            NavAgent.SetDestination(transform.position + (transform.forward * GEAI_Preset.GEAI_Options.LungeDistance));

            while (NavAgent.remainingDistance > .002f) //magic number :(
            {
                yield return new WaitForSeconds(_RoutineSleepDuration);
            }

            NavAgent.SetDestination(transform.position);
            NavAgent.speed = GEAI_Preset.Base.MovementSpeed;

            ChangeState(State.Idle);
        }

        #endregion
    }
}