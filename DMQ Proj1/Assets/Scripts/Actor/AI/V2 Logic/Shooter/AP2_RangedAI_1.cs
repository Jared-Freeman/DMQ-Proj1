using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using System.Linq;
using ActorSystem.AI;
using AbilitySystem;

namespace ActorSystem.AI
{
    public class AP2_RangedAI_1 : ActorAI_Logic
    {

        public Utils.CooldownTracker AttackCooldown;
        public AP2_ActorAction_AttackTarget AttackAction;



        //state vars
        public State CurrentState { get; protected set; }

        protected AP2RAIStateInfo AP2RAI_Info = new AP2RAIStateInfo();

        public override ActorAI_Logic_PresetBase Preset 
        { 
            get => base.Preset as AP2_RangedAI_1_LogicPreset; 
            set => base.Preset = value as AP2_RangedAI_1_LogicPreset; 
        }
        protected AP2_RangedAI_1_LogicPreset RAI_Preset
        {
            get { return Preset as AP2_RangedAI_1_LogicPreset; }
            set { Preset = value as AP2_RangedAI_1_LogicPreset; }
        }

        #region Helpers
        //internal helper
        protected class AP2RAIStateInfo
        {
            /// <summary>
            /// Allows us to mark a location using Transform for the base turn interpolator
            /// </summary>
            public GameObject CustomTurnTargetCookie;

            public Vector3 Reposition_DesiredPosition 
            { 
                get { return CustomTurnTargetCookie.transform.position; } 
                set { CustomTurnTargetCookie.transform.position = value; } 
            }

            public int Attack_AttacksInvoked = 0;

            public float ChargeAttack_StartTime = 0;

            public AS_Ability_Instance_Base _AttackAbility;
        }

        public enum State { Idle, Repositioning, TurningToFaceTarget, ChargingAttack, Attacking }
        #endregion


        #region Init
        protected override void Awake()
        {
            base.Awake();

            AP2RAI_Info._AttackAbility = RAI_Preset.RAI_Options.AttackAbility.GetInstance(gameObject);
            AP2RAI_Info.CustomTurnTargetCookie = new GameObject("[AI] Turning Cookie");
            AP2RAI_Info.CustomTurnTargetCookie.transform.position = transform.position;

            //Overrides of NavAgent properties
            NavAgent.updateRotation = false;
            NavAgent.speed = Preset.Base.MovementSpeed;
        }

        protected override void Start()
        {
            base.Start();

            ChangeState(State.Idle);
            StartCoroutine(UpdateAI());
            Info.CanTurn = true;
            //StartCoroutine(I_AdjustFacing());
        }
        #endregion

        protected virtual void OnDestroy()
        {
            Destroy(AP2RAI_Info.CustomTurnTargetCookie);
        }

        private void OnDrawGizmos()
        {
            if (Preset.Base.DrawDebugGizmos)
            {
                Gizmos.DrawWireSphere(transform.position, Preset.Base.AggroRadius);

                Gizmos.DrawWireSphere(transform.position, Preset.Base.AttackRange);

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
                    TurnTarget = AP2RAI_Info.CustomTurnTargetCookie.transform;
                    AP2RAI_Info.CustomTurnTargetCookie.transform.position = transform.position + transform.forward;
                    break;


                case State.Repositioning:
                    TurnTarget = AP2RAI_Info.CustomTurnTargetCookie.transform;
                    Reposition_ChooseDesiredPosition();
                    break;

                case State.TurningToFaceTarget:
                    TurnTarget = Info.CurrentTarget.transform;
                    break;

                case State.ChargingAttack:
                    TurnTarget = Info.CurrentTarget.transform; //TODO: Add handler to do trajectory projection. Maybe base difficulty on this??
                    AP2RAI_Info.ChargeAttack_StartTime = Time.time;
                    StartCoroutine(I_IncreaseScale());
                    break;

                case State.Attacking:
                    TurnTarget = AP2RAI_Info.CustomTurnTargetCookie.transform;
                    AP2RAI_Info.CustomTurnTargetCookie.transform.position = transform.position + transform.forward;

                    AP2RAI_Info.Attack_AttacksInvoked = 0;
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
                Info.CanMove = 
                    (
                    Vector3.Angle(gameObject.transform.forward, (TurnTarget.transform.position - gameObject.transform.position).normalized)
                    < Preset.Base.MaxFacingAngle / 2
                    );

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
            if (!AP2RAI_Info._AttackAbility.CanCastAbility) return; //TODO: Make this ability decision tree more intelligent. For now we just wait until ability works

            if (AP2RAI_Info.Attack_AttacksInvoked < 1)
            {
                if (FLAG_Debug) Debug.Log("ATTACKING " + CurrentTarget.name);

                EffectTree.EffectContext effectContext = new EffectTree.EffectContext();
                effectContext.AttackData = new Utils.AttackContext();
                {
                    var c = effectContext.AttackData;
                    c._InitialDirection = transform.forward;
                    c._InitialGameObject = gameObject;
                    c._InitialPosition = transform.position;
                    c._Owner = AttachedActor;
                    c._TargetDirection = transform.forward;
                    c._TargetGameObject = CurrentTarget;
                    c._TargetPosition = CurrentTarget.transform.position;
                    c._Team = AttachedActor._Team;
                }
                AP2RAI_Info._AttackAbility.ExecuteAbility(ref effectContext);
                //AttackAction.AttackTarget(AttachedActor, CurrentTarget);
                AP2RAI_Info.Attack_AttacksInvoked++;
            }
            else
            {
                ChangeState(State.Repositioning);
            }
        }

        private void ContinueChargeAttack()
        {
            if (CurrentTarget == null) ChangeState(State.Idle);

            if (Mathf.Abs(AP2RAI_Info.ChargeAttack_StartTime - Time.time) > RAI_Preset.RAI_Options.AttackChargeTime)
            {
                ChangeState(State.Attacking);
            }
        }

        private void ContinueReposition()
        {
            if (CurrentTarget == null) ChangeState(State.Idle);

            if (FLAG_Debug) Debug.DrawRay(AP2RAI_Info.Reposition_DesiredPosition, Vector3.up * 4f, Color.red, _RoutineSleepDuration);

            //Evaluate current desired movement point. Is it still in attack range?
            float DistSquared = (CurrentTarget.transform.position - AP2RAI_Info.Reposition_DesiredPosition).sqrMagnitude;
            if (DistSquared > Mathf.Pow(Preset.Base.AttackRange, 2) || DistSquared < Mathf.Pow(RAI_Preset.RAI_Options.AttackRangeMin, 2))
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
            if (Utils.AI.RandomPointInCircle(CurrentTarget.transform.position, RAI_Preset.RAI_Options.AttackRangeMin, Preset.Base.AttackRange, out Pos))
            {
                AP2RAI_Info.Reposition_DesiredPosition = Pos;
                NavAgent.SetDestination(AP2RAI_Info.Reposition_DesiredPosition);

            }
            else
            {
                AP2RAI_Info.Reposition_DesiredPosition = transform.position;
                if (FLAG_Debug) Debug.Log("No good reposition found.");
            }
        }

        #region Utility Methods

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
            while (CurrentState == State.ChargingAttack && Time.time - StartTime < RAI_Preset.RAI_Options.AttackChargeTime)
            {
                transform.localScale = DefaultScale * Preset.Base.GrowCurve.Evaluate((Time.time - StartTime) / RAI_Preset.RAI_Options.AttackChargeTime);
                yield return null;
            }

            transform.localScale = DefaultScale;
        }

        #endregion
    }


}