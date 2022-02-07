using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using System.Linq;
using ActorSystem.AI;

namespace ActorSystem.AI
{
    public class AP2_RangedAI_1 : ActorAI_Logic
    {

        public Utils.CooldownTracker AttackCooldown;
        public AP2_ActorAction_AttackTarget AttackAction;

        public AIOptions Options;


        //state vars
        public State CurrentState { get; protected set; }
        public GameObject CurrentTarget { get; protected set; }

        StateInfo Info = new StateInfo();



        #region Helpers
        //internal helper
        private class StateInfo
        {
            public List<Coroutine> ActiveRoutines = new List<Coroutine>();

            public bool CanTurn = true;

            public Vector3 Reposition_DesiredPosition = Vector3.zero;

            public int Attack_AttacksInvoked = 0;

            public float ChargeAttack_StartTime = 0;
        }

        //inspector helper
        [System.Serializable]
        public class AIOptions
        {
            public bool DrawDebugGizmos = false;

            public Utils.CooldownTracker AttackCooldown = new Utils.CooldownTracker();

            [Min(0f)]
            public float AttackRange = 1.25f; //max distance AI can be from target
            [Min(0f)]
            public float AttackRangeMinimum = 0f; //min dist AI must be from target to decide to attack

            [Min(0f)]
            public float AttackChargeTime = 1.25f; //max distance AI can be from target

            public float AggroRadius = 20;


            public AnimationCurve GrowCurve;

            public TargetPriority _TargetPriority = TargetPriority.Proximity;
        }

        public enum State { Idle, Repositioning, TurningToFaceTarget, ChargingAttack, Attacking }
        #endregion


        #region Init
        protected override void Awake()
        {
            base.Awake();

            Options.AttackCooldown.InitializeCooldown();

            //Overrides of NavAgent properties
            NavAgent.updateRotation = false;
            NavAgent.speed = Preset.Base.MovementSpeed;
        }

        protected override void Start()
        {
            base.Start();

            ChangeState(State.Idle);
            StartCoroutine(UpdateAI());
            StartCoroutine(I_AdjustFacing());
        }
        #endregion


        private void OnDrawGizmos()
        {
            if (Options.DrawDebugGizmos)
            {
                Gizmos.DrawWireSphere(transform.position, Options.AggroRadius);

                Gizmos.DrawWireSphere(transform.position, Options.AttackRange);

                if (CurrentTarget != null) Gizmos.DrawRay(CurrentTarget.transform.position, Vector3.up * 4f);
            }
        }


        //Can INIT stuff at start of state (in the switch block)
        private void ChangeState(State Next_State)
        {
            EndState(CurrentState);
            CurrentState = Next_State;

            if (FLAG_Debug) Debug.Log("STATE CHANGE: " + CurrentState.ToString());

            switch (CurrentState)
            {
                case State.Idle:
                    break;


                case State.Repositioning:
                    Reposition_ChooseDesiredPosition();
                    break;

                case State.TurningToFaceTarget:
                    break;

                case State.ChargingAttack:
                    Info.ChargeAttack_StartTime = Time.time;
                    StartCoroutine(I_IncreaseScale());
                    break;



                case State.Attacking:
                    Info.Attack_AttacksInvoked = 0;
                    break;

                default:
                    Debug.LogError(ToString() + ": Unrecognized AI State!");
                    break;
            }
        }

        //Allows us to perform stuff on end of state
        private void EndState(State StateToEnd)
        {
            switch (StateToEnd)
            {
                case State.Idle:
                    break;

                case State.Repositioning:
                    break;

                case State.TurningToFaceTarget:
                    break;

                case State.ChargingAttack:
                    break;

                case State.Attacking:
                    break;

                default:
                    Debug.LogError(ToString() + ": Unrecognized AI State!");
                    break;
            }
        }

        private IEnumerator UpdateAI()
        {
            while (true)
            {
                switch (CurrentState)
                {
                    case State.Idle:
                        ContinueSearchForTargets();
                        break;

                    case State.Repositioning:
                        ContinueReposition();
                        break;

                    case State.TurningToFaceTarget:
                        ContinueTurnToFaceTarget();
                        break;


                    case State.ChargingAttack:
                        ContinueChargeAttack();
                        break;

                    case State.Attacking:
                        ContinueAttack();
                        //Attack the target
                        break;

                    default:
                        Debug.LogError("AP2_GenericEnemyAI: Unrecognized AI State!");
                        break;
                }

                yield return new WaitForSeconds(_RoutineSleepDuration);
            }
        }

        private void ContinueTurnToFaceTarget()
        {
            if (Vector3.Angle(gameObject.transform.forward, (CurrentTarget.transform.position - gameObject.transform.position).normalized) < Preset.Base.MaxFacingAngle / 2)
            {
                ChangeState(State.ChargingAttack);
            }
        }


        private void ContinueSearchForTargets()
        {
            if (EnemyExistsInAggroRadius())
            {
                ChooseNewTarget();
                ChangeState(State.Repositioning);
            }
        }

        private void ContinueAttack()
        {
            if (CurrentTarget == null) ChangeState(State.Idle);

            if (Info.Attack_AttacksInvoked < 1)
            {
                if (FLAG_Debug) Debug.Log("ATTACKING " + CurrentTarget.name);
                AttackAction.AttackTarget(AttachedActor, CurrentTarget);
                Info.Attack_AttacksInvoked++;
            }
            else
            {
                ChangeState(State.Repositioning);
            }
        }

        private void ContinueChargeAttack()
        {
            if (CurrentTarget == null) ChangeState(State.Idle);

            if (Mathf.Abs(Info.ChargeAttack_StartTime - Time.time) > Options.AttackChargeTime)
            {
                ChangeState(State.Attacking);
            }
        }

        private void ContinueReposition()
        {
            if (CurrentTarget == null) ChangeState(State.Idle);

            if (FLAG_Debug) Debug.DrawRay(Info.Reposition_DesiredPosition, Vector3.up * 4f, Color.red, _RoutineSleepDuration);

            //Evaluate current desired movement point. Is it still in attack range?
            float DistSquared = (CurrentTarget.transform.position - Info.Reposition_DesiredPosition).sqrMagnitude;
            if (DistSquared > Options.AttackRange * Options.AttackRange || DistSquared < Options.AttackRangeMinimum * Options.AttackRangeMinimum)
            {
                Reposition_ChooseDesiredPosition();
            }
            else if (NavAgent.remainingDistance < .01f)
            {
                ChangeState(State.TurningToFaceTarget);
            }
        }

        private void Reposition_ChooseDesiredPosition()
        {
            //Random point between minrange and maxrange
            Vector3 Pos;
            if (Utils.AI.RandomPointInCircle(CurrentTarget.transform.position, Options.AttackRangeMinimum, Options.AttackRange, out Pos))
            {
                Info.Reposition_DesiredPosition = Pos;
                NavAgent.SetDestination(Info.Reposition_DesiredPosition);

            }
            else
            {
                Info.Reposition_DesiredPosition = transform.position;
                if (FLAG_Debug) Debug.Log("No good reposition found.");
            }
        }

        #region Utility Methods
        //TODO: Consider optimizing
        public bool EnemyExistsInAggroRadius()
        {
            foreach (Actor A in Singleton<ActorManager>.Instance.ActorList)
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

            switch (Options._TargetPriority)
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
                    if (Singleton<ActorManager>.Instance.ActorList == null)
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
                    if (ProximalActors.Count > 0)
                    {
                        ProximalActors.OrderBy(t => (t.gameObject.transform.position - gameObject.transform.position).sqrMagnitude);
                        CurrentTarget = ProximalActors[0].gameObject;
                    }

                    break;


                default:
                    throw new System.NotImplementedException("No impl exits for TargetPriority: " + Options._TargetPriority.ToString());
            }
        }


        protected IEnumerator I_AdjustFacing()
        {
            bool FLAG_Done = false;
            while (!FLAG_Done && Info.CanTurn)
            {
                switch (CurrentState)
                {
                    //TODO: Maybe a random look behavior...
                    case State.Idle:
                        break;


                    //face destination
                    case State.Repositioning:
                        {
                            float Angle = Vector3.SignedAngle(gameObject.transform.forward, (NavAgent.destination - gameObject.transform.position).normalized, Vector3.up);

                            float ScaledTurnRate = Preset.Base.TurningRate * Time.deltaTime;

                            var Rot = Vector3.ProjectOnPlane((NavAgent.destination - transform.position), Vector3.up);

                            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(Rot, Vector3.up), ScaledTurnRate);
                        }
                        break;


                    //face target
                    case State.TurningToFaceTarget:
                    case State.ChargingAttack:
                        if (CurrentTarget != null)
                        {
                            float Angle = Vector3.SignedAngle(gameObject.transform.forward, (CurrentTarget.transform.position - gameObject.transform.position).normalized, Vector3.up);

                            float ScaledTurnRate = Preset.Base.TurningRate * Time.deltaTime;

                            var Rot = Vector3.ProjectOnPlane((CurrentTarget.transform.position - transform.position), Vector3.up);

                            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(Rot, Vector3.up), ScaledTurnRate);
                        }
                        break;


                    //no facing adjusting
                    case State.Attacking:
                        break;

                    default:
                        Debug.LogError("AP2_GenericEnemyAI: Unrecognized AI State!");
                        break;
                }

                yield return null;
            }
        }

        private IEnumerator I_IncreaseScale()
        {
            var DefaultScale = transform.localScale;

            float StartTime = Time.time;
            while (CurrentState == State.ChargingAttack && Time.time - StartTime < Options.AttackChargeTime)
            {
                transform.localScale = DefaultScale * Options.GrowCurve.Evaluate((Time.time - StartTime) / Options.AttackChargeTime);
                yield return null;
            }

            transform.localScale = DefaultScale;
        }

        #endregion
    }


}