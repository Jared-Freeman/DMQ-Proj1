using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace AP2
{
    //This class is intended to simply walk to a point near the player, lunge, and perform an attack at the end of the lunge
    public class AP2_GenericEnemyAI : ActorAI_Logic
    {
        private static float _RoutineSleepDuration = .125f; //8 times / sec

        public bool FLAG_Debug = false;

        public Utils.CooldownTracker AttackCooldown;
        public AP2.AP2_ActorAction_AttackTarget AttackAction;
        
        public AIOptions Options;


        //state vars
        public State CurrentState { get; protected set; }
        private GameObject CurrentTarget;

        StateInfo Info = new StateInfo();
        


        #region Helpers
        //internal helper
        private class StateInfo
        {
            public List<Coroutine> ActiveRoutines = new List<Coroutine>();

            public float LungeStartTime = 0f;
            public bool CanTurn = false;
            public int CurrentAttacksInvoked = 0;
        }

        //inspector helper
        [System.Serializable]
        public class AIOptions
        {
            public bool DrawDebugGizmos = false;

            [Min(0f)]
            public float AttackRange = 1.25f;

            public float MovementSpeed = 2f; //TODO: Currently not hooked up to navmesh agent
            public float StopSlideDistance = .5f;
                       
            public float AggroRadius = 20;

            [Tooltip("Max angle the agent can move toward without needing to stop and turn")]
            public float MaxFacingAngle = 250f;

            [Min(0f)]
            public float LungePrepareDistance = 2f;
            public float LungeLosePrepareDistance = 3f;
            public float LungeDistance = 3f;
            [Min(0f)]
            public float LungePause = 2f;
            [Min(0f)]
            public float LungeSpeed = 8f;
            [Min(0f)]
            public float LungeTimeout = 1.25f;

            [Tooltip("Deg/sec")]
            public float TurningRate = 360f;

            public AnimationCurve GrowCurve;
            public float GrowDuration = .5f;

            public TargetPriority _TargetPriority = TargetPriority.Proximity;
        }

        public enum State { Idle, Chasing, PrepToLunge, Lunging, Attacking }
        public enum TargetPriority { None, Proximity }
        #endregion


        #region Init
        protected override void Awake()
        {
            base.Awake();

            AttackCooldown.InitializeCooldown();

            //Overrides of NavAgent properties
            NavAgent.updateRotation = false;
            NavAgent.speed = Options.MovementSpeed;
        }

        protected override void Start()
        {
            base.Start();

            ChangeState(State.Idle);
            StartCoroutine(UpdateAI());
            StartCoroutine(I_TurnInterpolate());
        }
        #endregion


        private void OnDrawGizmos()
        {
            if(Options.DrawDebugGizmos)
            {
                Gizmos.DrawWireSphere(transform.position, Options.AggroRadius);

                Gizmos.DrawWireSphere(transform.position, Options.LungePrepareDistance);

                if (CurrentTarget != null) Gizmos.DrawRay(CurrentTarget.transform.position, Vector3.up * 4f);
            }
        }


        private IEnumerator UpdateAI()
        {
            foreach (Coroutine C in Info.ActiveRoutines)
            {
                if (C == null && FLAG_Debug) Debug.Log("Routine not found!!!");
                else StopCoroutine(C);
            }

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
                    NavAgent.SetDestination(transform.position + (transform.forward * Options.StopSlideDistance));
                    Info.LungeStartTime = Time.time;
                    StartCoroutine(I_IncreaseScale());
                    break;

                case State.Lunging:
                    //new attack
                    Info.CurrentAttacksInvoked = 0;
                    //disable turning
                    Info.CanTurn = false;
                    NavAgent.speed = Options.LungeSpeed;
                    NavAgent.SetDestination(transform.position + (transform.forward * (Options.LungeDistance + Options.StopSlideDistance)));
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
                    && collision.rigidbody.gameObject == CurrentTarget
                    && (CurrentTarget.transform.position - transform.position).sqrMagnitude < Options.AttackRange * Options.AttackRange) //can probably remove w new syst
                {
                    if (FLAG_Debug) Debug.Log("ATTACKING " + CurrentTarget.name);
                    AttackAction.AttackTarget(AttachedActor, CurrentTarget);
                    Info.CurrentAttacksInvoked++;
                }

            }
        }


        #region Utility Routines
        private IEnumerator I_IncreaseScale()
        {
            var DefaultScale = transform.localScale;

            float StartTime = Time.time;
            while (CurrentState == State.PrepToLunge && Time.time - StartTime < Options.GrowDuration )
            {
                transform.localScale = DefaultScale * Options.GrowCurve.Evaluate((Time.time - StartTime) / Options.GrowDuration);
                yield return null;
            }

            transform.localScale = DefaultScale;
        }

        private IEnumerator I_TurnInterpolate()
        {
            bool FLAG_Done = false;
            while (!FLAG_Done)
            {
                if (Info.CanTurn && CurrentTarget != null)
                {
                    float Angle = Vector3.SignedAngle(gameObject.transform.forward, (CurrentTarget.transform.position - gameObject.transform.position).normalized, Vector3.up);

                    float ScaledTurnRate = Options.TurningRate * Time.deltaTime;

                    var Rot = Vector3.ProjectOnPlane((CurrentTarget.transform.position - transform.position), Vector3.up);

                    transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(Rot, Vector3.up), ScaledTurnRate);
                }

                yield return null;
            }
        }
        #endregion

        #region Utility Methods
        //TODO: Consider optimizing
        public bool EnemyExistsInAggroRadius()
        {
            foreach(Actor A in Singleton<ActorManager>.Instance.ActorList)
            {
                if (
                    (A.gameObject.transform.position - gameObject.transform.position).sqrMagnitude <= (Options.AggroRadius * Options.AggroRadius) 
                    && A._Team.IsEnemy(AttachedActor._Team)
                    )
                {
                    if (FLAG_Debug) Debug.Log("Enemy in Aggro Range");
                    return true;
                }
            }
            return false;
        }

        private void ChooseNewTarget()
        {
            //TODO: consider some more robust logic here. Could have the AI decide based on some criteria like hp remaining or threat.
            //For now proximity will do

            switch(Options._TargetPriority)
            {
                case TargetPriority.None:
                    //get random from selection
                    foreach (Actor A in Singleton<ActorManager>.Instance.ActorList)
                    {
                        if (
                            (A.gameObject.transform.position - gameObject.transform.position).sqrMagnitude <= (Options.AggroRadius * Options.AggroRadius)
                            && A._Team.IsEnemy(AttachedActor._Team)
                            )
                        {
                            CurrentTarget = A.gameObject; //possible multiple reassignment... but it doesnt matter here
                        }
                    }
                    break;


                case TargetPriority.Proximity:

                    List<Actor> ProximalActors = new List<Actor>();
                    if(Singleton<ActorManager>.Instance.ActorList == null)
                    {
                        Debug.LogError("WAT");
                        return;
                    }

                    foreach (Actor A in Singleton<ActorManager>.Instance.ActorList)
                    {
                        if (
                            (A.gameObject.transform.position - gameObject.transform.position).sqrMagnitude <= (Options.AggroRadius * Options.AggroRadius)
                            && A._Team.IsEnemy(AttachedActor._Team)
                            )
                        {
                            ProximalActors.Add(A);
                        }
                    }
                    if(ProximalActors.Count > 0)
                    {
                        ProximalActors.OrderBy(t => (t.gameObject.transform.position - gameObject.transform.position).sqrMagnitude);
                        CurrentTarget = ProximalActors[0].gameObject;
                    }

                    break;


                default:
                    throw new System.NotImplementedException("No impl exits for TargetPriority: " + Options._TargetPriority.ToString());
            }
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
            if(CurrentTarget == null)
            {
                ChangeState(State.Idle);
            }
            else if (
                (CurrentTarget.transform.position - transform.position).sqrMagnitude <= Options.LungePrepareDistance * Options.LungePrepareDistance
                && (NavAgent.path.corners.Length < 3) //straight shot
                )
            {
                ChangeState(State.PrepToLunge);
            }
            else if (Vector3.Angle(gameObject.transform.forward, (CurrentTarget.transform.position - gameObject.transform.position).normalized) <= Options.MaxFacingAngle / 2)
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
            else if ((CurrentTarget.transform.position - transform.position).sqrMagnitude >= Options.LungeLosePrepareDistance * Options.LungeLosePrepareDistance)
            {
                ChangeState(State.Chasing);
            }
            else if (Vector3.Angle(gameObject.transform.forward, (CurrentTarget.transform.position - gameObject.transform.position).normalized) > Options.MaxFacingAngle / 2)
            {
                ChangeState(State.Chasing);
            }
            else if(NavAgent.CalculatePath(CurrentTarget.transform.position, NavPath) && NavPath.corners.Length > 2)
            {
                ChangeState(State.Chasing);
            }
            else if(Time.time - Info.LungeStartTime > Options.LungePause)
            {
                ChangeState(State.Lunging);
            }
        }

        private void Lunge()
        {

            if (NavAgent.remainingDistance < .002f || (Time.time - Info.LungeStartTime) > Options.LungeTimeout) //magic number :(
            {
                NavAgent.SetDestination(transform.position);
                NavAgent.speed = Options.MovementSpeed;
                ChangeState(State.Chasing);
            }
        }
        #endregion



        //I'm still a bit mad that this solution concept doesnt work and I (naively) blame Unity
        #region Dumpstered Methods

        private void OldChangeState(State CurState)
        {
            //apparently you can ONLY stop coroutines when a string is supplied to make them?? Thanks unity...
            foreach (Coroutine C in Info.ActiveRoutines)
            {
                if (C == null) Debug.LogError("Routine not found!!!");
                else StopCoroutine(C.ToString());
            }
            StopAllCoroutines();


            Info.ActiveRoutines.Clear();

            CurrentState = CurState;

            if (FLAG_Debug) Debug.Log("STATE CHANGE: " + CurState.ToString());

            switch (CurState)
            {
                case State.Idle:
                    //Look for a bad guy to chase
                    Info.ActiveRoutines.Add(StartCoroutine("I_SearchForTargets"));
                    break;

                case State.Chasing:
                    //Consider checking for a better target here at some point
                    //Check for target death (subscribe to an event?)
                    //Get in attacking range of current Target

                    //turning needs to be interpolated per-frame. Added it to current routines stack
                    Info.ActiveRoutines.Add(StartCoroutine("I_TurnInterpolate"));
                    Info.ActiveRoutines.Add(StartCoroutine("I_ChaseCurrentTarget"));
                    break;

                case State.PrepToLunge:
                    //Brief pause before Lunging. Target may exit lunge range in this time. Gives target the chance to dodge or disengage

                    //turning needs to be interpolated per-frame. Added it to current routines stack
                    Info.ActiveRoutines.Add(StartCoroutine("I_TurnInterpolate"));
                    Info.ActiveRoutines.Add(StartCoroutine("I_WaitToLunge"));
                    break;

                case State.Lunging:
                    //Sprint the last little distance
                    Info.ActiveRoutines.Add(StartCoroutine("I_Lunge"));
                    break;

                case State.Attacking:
                    //Attack the target
                    break;

                default:
                    Debug.LogError("AP2_GenericEnemyAI: Unrecognized AI State!");
                    break;
            }
        }

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

            if ((CurrentTarget.transform.position - transform.position).sqrMagnitude <= Options.LungePrepareDistance * Options.LungePrepareDistance) CanLunge = true;

            while (CurrentTarget != null && !CanLunge)
            {
                if ((CurrentTarget.transform.position - transform.position).sqrMagnitude <= Options.LungePrepareDistance * Options.LungePrepareDistance)
                {
                    if (FLAG_Debug) Debug.DrawRay(gameObject.transform.position, gameObject.transform.up * 6f, Color.yellow, _RoutineSleepDuration);

                    NavAgent.SetDestination(transform.position);

                    CanLunge = true;


                    ////only turning
                    //NavAgent.updatePosition = false;
                    //NavAgent.SetDestination((CurrentTarget.transform.position - gameObject.transform.position).normalized);
                }
                else if (Vector3.Angle(gameObject.transform.forward, (CurrentTarget.transform.position - gameObject.transform.position).normalized) <= Options.MaxFacingAngle / 2)
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
                && Mathf.Abs(CurTime - StartTime) < Options.LungePause
                && (CurrentTarget.transform.position - transform.position).sqrMagnitude <= Options.LungeLosePrepareDistance * Options.LungeLosePrepareDistance
                && (Vector3.Angle(gameObject.transform.forward, (CurrentTarget.transform.position - gameObject.transform.position).normalized) <= Options.MaxFacingAngle / 2)
                )
            {
                CurTime = Time.time;

                //if (FLAG_Debug)
                //{
                //    Debug.Log("Duration Complete: " + (Mathf.Abs(CurTime - StartTime) < Options.LungePause).ToString());
                //    Debug.Log("Angle Complete: " + ((Vector3.Angle(gameObject.transform.forward, (CurrentTarget.transform.position - gameObject.transform.position).normalized) <= Options.MaxFacingAngle / 2)).ToString());

                //    Debug.Log("Distance Complete: " + ((CurrentTarget.transform.position - transform.position).sqrMagnitude <= Options.LungeLosePrepareDistance * Options.LungeLosePrepareDistance).ToString());
                //}

                yield return new WaitForSeconds(_RoutineSleepDuration);
            }

            if (CurrentTarget == null)
                ChangeState(State.Idle);

            else if ((CurrentTarget.transform.position - transform.position).sqrMagnitude > Options.LungeLosePrepareDistance * Options.LungeLosePrepareDistance)
                ChangeState(State.Chasing);

            else
                ChangeState(State.Lunging);

        }
        private IEnumerator I_Lunge()
        {
            NavAgent.speed = Options.LungeSpeed;
            NavAgent.SetDestination(transform.position + (transform.forward * Options.LungeDistance));

            while (NavAgent.remainingDistance > .002f) //magic number :(
            {
                yield return new WaitForSeconds(_RoutineSleepDuration);
            }

            NavAgent.SetDestination(transform.position);
            NavAgent.speed = Options.MovementSpeed;

            ChangeState(State.Idle);
        }

        #endregion
    }


}