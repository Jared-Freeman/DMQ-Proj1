using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.AI;

using ActorSystem.AI;

//This class holds the reigns of any AI. Its goal is to take the ActorAI data structure and drive it during game simulation.
[RequireComponent(typeof(ActorAI))]
[RequireComponent(typeof(NavMeshAgent))]
public class ActorAI_Logic : MonoBehaviour
{
    public enum TargetPriority { None, Proximity }
    /// <summary>
    /// Enum containing all possible states for every AI, ever. 
    /// </summary>
    /// <remarks>
    ///     <para>
    ///     Add to the end of this enum to append new states.
    ///     </para>
    ///     <para>
    ///     Not every state should be implemented in every AI Logic. 
    ///     </para>
    /// </remarks>
    public enum ActorAILogic_State { Idle, Repositioning, TurningToFaceTarget, ChargingAttack, Attacking, Chasing, PreparingToLunge, Lunging }

    /// <summary>
    /// For Logic subroutines (minimize cost-per-frame)
    /// </summary>
    protected readonly static float _RoutineSleepDuration = .125f; //8 times / sec
    protected readonly static float s_MinimumRBSpeedToIntercept = 6;
    /// <summary>
    /// A property to introduce fuzziness to prevent P.F. priority fighting if we're close enough to our current destination
    /// </summary>
    protected readonly static float s_CloseEnoughToTarget = .5f;

    #region Members

    //flags
    public bool FLAG_Debug = false;

    //refs and state info
    protected StateInfo Info { get; set; } = new StateInfo();
    protected FlockingStateInfo FlockingInfo { get; set; } = new FlockingStateInfo();
    [SerializeField]
    private ActorAI_Logic_PresetBase _Preset;
    [SerializeField]
    private ActorSystem.AI.Flocking.ActorAI_FlockingParametersPreset _FlockingPreset;

    #region Properties

    /// <summary>
    /// The current state in this AI Logic's internal state machine.
    /// </summary>
    public ActorAILogic_State CurrentState = ActorAILogic_State.Idle;
    /// <summary>
    /// Virtual Property that can be overwritten to enforce subclassing
    /// </summary>
    public virtual ActorAI_Logic_PresetBase Preset { get { return _Preset; } set { _Preset = value; } }
    /// <summary>
    /// Current Aggro target
    /// </summary>
    public GameObject CurrentTarget 
    { 
        get 
        {
            return Info.CurrentTarget;
        } 
        protected set 
        { 
            Info.CurrentTarget = value;

            //also update rigidbody record
            var rb = Info.CurrentTarget.GetComponent<Rigidbody>();
            if (rb != null)
            {
                CurrentTargetRigidbody = rb;
            }
            else CurrentTargetRigidbody = null;
        } 
    }
    public Rigidbody CurrentTargetRigidbody { get; private set; }
    public Vector3 CurrentTargetPosition
    {
        get
        {
            //a bunch of conditions that preclude intercept computation lol
            if (!Preset.Base.InterceptCurrentTarget || CurrentTargetRigidbody == null) return CurrentTarget.transform.position;
            if (CurrentTargetRigidbody.velocity.sqrMagnitude < Mathf.Pow(s_MinimumRBSpeedToIntercept + NavAgent.radius, 2)) return CurrentTarget.transform.position;
            if (CurrentTargetRigidbody.velocity.sqrMagnitude < Mathf.Pow(Preset.Base.InterceptCurrentTargetDisableDistance, 2)) return CurrentTarget.transform.position;

            //iff all checks didn't return, NOW we do intercept logic
            return CurrentTarget.transform.position + Info.PersonalInterceptPreference * Info.CurrentMovementTargetIntercept_Fuzzy;
        }
    }
    public Transform TurnTarget { get { return Info.TurnTarget; } protected set { Info.TurnTarget = value; } }
    public ActorAI AttachedActor { get; protected set; }
    public NavMeshAgent NavAgent { get; protected set; }
    public Animator Animator { get; protected set; }
    public bool CanMove { get { return Info.CanMove; } }
    public bool CanTurn { get { return Info.CanTurn; } }
    /// <summary>
    /// Returns true when Actor is within the facing tolerances specified by the Preset
    /// </summary>
    public bool IsFacingTarget
    {
        get
        {
            if(CurrentTarget != null)
                return (Vector3.Angle(gameObject.transform.forward, (CurrentTarget.transform.position - gameObject.transform.position).normalized) <= Preset.Base.MaxFacingAngle / 2);
            else return false;
        }
    }
    public Vector3 DesiredVelocity { get; protected set; }

    #endregion

    #region Helper data

    //internal helper
    protected class StateInfo
    {
        /// <summary>
        /// Acts as a kill switch do disable flocking movement influence should an AI need that behavior such as <see cref="AP2_GenericEnemyAI"/> charging routine.
        /// </summary>
        public bool FlockingEnabled = true;

        public float LungeStartTime = 0f; //TODO: move
        public bool CanTurn = false;
        public bool CanMove = true;
        public int CurrentAttacksInvoked = 0; //TODO: move

        /// <summary>
        /// Aggro target. Usually an Actor but not necessarily
        /// </summary>
        public GameObject CurrentTarget;

        /// <summary>
        /// A dummy you can assign the CurrentTarget to, and freely manipulate
        /// </summary>
        public GameObject PersonalWaypoint;
        /// <summary>
        /// Transform this Actor AI wants to turn toward
        /// </summary>
        public Transform TurnTarget;

        /// <summary>
        /// How much this INDIVIDUAL agent wishes to intercept it's target. Range [0,1]
        /// </summary>
        /// <remarks>
        /// If the preset option is enabled, this field will inialize to a value between 0 and the Preset maximum.
        /// </remarks>
        public float PersonalInterceptPreference = 0f;

        /// <summary>
        /// Interpolated velocity prediction
        /// </summary>
        public Vector3 CurrentMovementTargetIntercept_Fuzzy { get; set; } = Vector3.zero;

        //these prevent redundant costly math operations, namely the sqrt needed for the vector magnitude computation.
        //note these also preclude intercept computations... Sorry this will be confusing to others and my later self...
        public Vector3 DistanceToCurrentTarget_AtLastPoll = Vector3.zero;
        public float DistanceToCurrentTargetMagnitude_AtLastPoll = 0f;
    }

    /// <summary>
    /// The flocking contributions container
    /// </summary>
    [System.Serializable]
    protected class FlockingStateInfo
    {
        //MAGNITUDE contributions
        public Vector3 Avoidance;
        public Vector3 Alignment;
        public Vector3 Cohesion_Position; //this one is actually a position!
    }

    #endregion

    #endregion

    #region Initialization

    protected virtual void Awake()
    {
        AttachedActor = GetComponent<ActorAI>();
        NavAgent = GetComponent<NavMeshAgent>();

        //Ref Tests
        if (!Utils.Testing.ReferenceIsValid(AttachedActor)) Destroy(gameObject);
        if (!Utils.Testing.ReferenceIsValid(NavAgent)) Destroy(gameObject);
        if (!Utils.Testing.ReferenceIsValid(_FlockingPreset)) Destroy(gameObject);
        if (!Utils.Testing.ReferenceIsValid(_Preset)) Destroy(gameObject);

        Info.PersonalWaypoint = new GameObject("[AI] Personal Waypoint");
        Info.CurrentTarget = Info.PersonalWaypoint;
        Info.PersonalWaypoint.transform.position = transform.position;

        //introduces crowd noise, making movement more unpredictable depending on settings. Ideally we search for a middle ground between stability and predictability.
        Info.PersonalInterceptPreference = Mathf.Clamp(Random.Range(0,Preset.Base.InterceptCurrentTargetStrength), 0, 1);

        //Overrides of NavAgent properties
        NavAgent.updateRotation = false;
    }

    protected virtual void Start()
    {
        //only do per-frame interp if we DONT have a rigidbody active, or it's kinematic
        if (gameObject.GetComponent<Rigidbody>() == null) StartCoroutine(I_TurnInterpolate());
        else if (gameObject.GetComponent<Rigidbody>()?.isKinematic == true) StartCoroutine(I_TurnInterpolate());

    }

    protected virtual void OnEnable()
    {
        StartCoroutine(DumbNavMeshAgentFix());
    }

    protected IEnumerator DumbNavMeshAgentFix()
    {
        NavAgent.enabled = false;
        yield return new WaitForEndOfFrame(); //This hack delays the execution until the mesh is created
        NavAgent.enabled = true;
    }

    #endregion

    #region Update

    /// <summary>
    /// Rigidbody compliant turn interpolation... This solves stutter issue from the coroutine method
    /// </summary>
    void FixedUpdate()
    {
        if(CurrentTarget != null)
        {
            Info.DistanceToCurrentTarget_AtLastPoll = transform.position - CurrentTarget.transform.position;
            Info.DistanceToCurrentTargetMagnitude_AtLastPoll = Info.DistanceToCurrentTarget_AtLastPoll.magnitude;
        }
        else
        {
            Info.DistanceToCurrentTarget_AtLastPoll = Vector3.zero;
            Info.DistanceToCurrentTargetMagnitude_AtLastPoll = 0f;
        }

        var RB = gameObject.GetComponent<Rigidbody>();

        if (RB != null && RB.isKinematic == false && Info.CanTurn && TurnTarget != null)
        {
            float Angle = Vector3.SignedAngle(gameObject.transform.forward, (TurnTarget.transform.position - gameObject.transform.position).normalized, Vector3.up);

            float ScaledTurnRate = Preset.Base.TurningRate * Time.fixedDeltaTime;

            var Rot = Vector3.ProjectOnPlane((TurnTarget.transform.position - transform.position), Vector3.up);
            Quaternion QResult;
            if (Rot.sqrMagnitude != 0)
            {
                QResult = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(Rot, Vector3.up), ScaledTurnRate);
                RB.MoveRotation(QResult);
            }
            //else QResult = Quaternion.identity;

            //transform.rotation = QResult;

        }


        if(Preset.Base.InterceptCurrentTarget)
        {
            UpdateInterceptComputation(Mathf.Clamp(Time.fixedDeltaTime * 3.3f, 0, 1f));
        }

        if(Info.FlockingEnabled 
            && Info.DistanceToCurrentTargetMagnitude_AtLastPoll <= s_CloseEnoughToTarget
            && Info.DistanceToCurrentTargetMagnitude_AtLastPoll <= Preset.Base.InterceptCurrentTargetDisableDistance)
        {
            //can afford to do this here since we're using FixedTime for physic movement
            UpdateFlocking();
        }
        else
        {
            DesiredVelocity = NavAgent.desiredVelocity;
        }
    }

    protected virtual void Update()
    {
    }

    private void UpdateFlocking()
    {
        //DesiredVelocity = NavAgent.desiredVelocity; return;

        List<GameObject> ignoredGOs = new List<GameObject>();
        ignoredGOs.Add(gameObject);

        float searchRadius = 8f; //TODO: make this the max radius of the 3 params

        List<Actor> List_ProximalActors = Utils.ComponentFinder<Actor>.GetComponentsWithColliderInRadius(transform.position, searchRadius, ignoredGOs);

        Flocking_Avoidance(List_ProximalActors);

        //assemble final velocity contributions
        Vector3 v_desiredVelocity = NavAgent.desiredVelocity + _FlockingPreset.Options.OverallFlockingStrength * (FlockingInfo.Avoidance + FlockingInfo.Alignment);
        if(v_desiredVelocity.sqrMagnitude > Mathf.Pow(AttachedActor.Stats.MoveSpeedCurrent,2))
        {
            v_desiredVelocity = v_desiredVelocity.normalized * AttachedActor.Stats.MoveSpeedCurrent;
        }

        if (FLAG_Debug)
        {
            Debug.DrawRay(transform.position + new Vector3(0, .5f, 0), v_desiredVelocity, Color.white, Time.fixedDeltaTime);
        }

        //end goal, the NavAgent's desired movement is respected more as we get closer to target
        DesiredVelocity = v_desiredVelocity;
    }

    #region Flocking

    private void Flocking_Avoidance(List<Actor> proximalActors)
    {
        //filter out proximals that are too far away. Cheaper than doing multiple sphere overlaps.
        List<Actor> allowedProxActors = Flocking_GetSublistOfAllowableActors(proximalActors, _FlockingPreset.Options.Avoidance);

        if (allowedProxActors == null || allowedProxActors.Count < 1)
        {
            FlockingInfo.Avoidance = Vector3.zero;
            return;
        }

        //Here we use a reciprocal function to weight closer agents stronger than distant ones.
        float maxStrength = _FlockingPreset.Options.Avoidance.MaxStrength; // Strength when distance == 0. Useful for determining the theoretical maximum contribution.
        float steepness = _FlockingPreset.Options.Avoidance.CurveSteepness; // Steepness will make the strength taper more/less harshly.
                              // Low values look like a square wave (e.g. 0 to maxStrength instantly). High values look like a horizontal line at y=maxStrength

        Vector3 totalVelocityDesired = Vector3.zero;
        Vector3 curDistance;

        float v_denom_term2 = steepness * (1 / maxStrength); //term 2 in the denominator
        float aCount = allowedProxActors.Count; //normalizing term
        float spd = AttachedActor.Stats.MoveSpeedCurrent;
        float v_numerator = spd * steepness / aCount; //numerator
        //compute vector sum of all proximal actors using the avoidance function
        foreach (var a in allowedProxActors)
        {
            //if (FLAG_Debug)
            //{
            //    Debug.DrawRay(a.transform.position, new Vector3(0, 8f, 0), Color.red, Time.fixedDeltaTime);
            //}

            curDistance = a.transform.position - gameObject.transform.position;

            totalVelocityDesired += (v_numerator / (Info.DistanceToCurrentTargetMagnitude_AtLastPoll + v_denom_term2)) * curDistance.normalized;
        }

        totalVelocityDesired *= spd;

        //if (FLAG_Debug)
        //{
        //    Debug.DrawRay(transform.position + new Vector3(0, .5f, 0), -totalVelocityDesired, Color.cyan, Time.fixedDeltaTime);
        //    Debug.Log(totalVelocityDesired);
        //}

        FlockingInfo.Avoidance = -totalVelocityDesired;
    }
    private void Flocking_Cohesion(List<Actor> proximalActors)
    {

    }
    private void Flocking_Alignment(List<Actor> proximalActors)
    {
        //filter out proximals that are too far away. Cheaper than doing multiple sphere overlaps.
        List<Actor> allowedProxActors = Flocking_GetSublistOfAllowableActors(proximalActors, _FlockingPreset.Options.Alignment);

        if (allowedProxActors == null || allowedProxActors.Count < 1)
        {
            FlockingInfo.Alignment = Vector3.zero;
            return;
        }

        Vector3 resultVector = Vector3.zero;


        //determine an average velocity vector 

        //Here we use a reciprocal function to weight closer agents stronger than distant ones.
        float maxStrength = _FlockingPreset.Options.Alignment.MaxStrength; // Strength when distance == 0. Useful for determining the theoretical maximum contribution.
        float steepness = _FlockingPreset.Options.Alignment.CurveSteepness; // Steepness will make the strength taper more/less harshly.
                                // Low values look like a square wave (e.g. 0 to maxStrength instantly). High values look like a horizontal line at y=maxStrength

        Vector3 curVelocity;
        Vector3 curDistance;

        Vector3 externalTotalVelocity = Vector3.zero;

        float aCount = allowedProxActors.Count; //normalizing term

        float currentMaxMoveSpeed = AttachedActor.Stats.MoveSpeedCurrent;

        float v_numerator = steepness / aCount; //numerator
        float v_denom_term = steepness * (1 / maxStrength); //term 2 in the denominator

        Rigidbody curRB;
        float curCoef;
        float totalCoef = 1f;
        //compute vector sum of all proximal actors using the avoidance function
        foreach (var a in allowedProxActors)
        {
            //if (FLAG_Debug)
            //{
            //    Debug.DrawRay(a.transform.position, new Vector3(0, 8f, 0), Color.red, Time.fixedDeltaTime);
            //}

            curRB = a.GetComponent<Rigidbody>();

            if(curRB != null)
            {
                curVelocity = curRB.velocity;
                curDistance = a.transform.position - gameObject.transform.position;

                curCoef = (v_numerator / (Info.DistanceToCurrentTargetMagnitude_AtLastPoll + v_denom_term));

                externalTotalVelocity += curRB.velocity;
                resultVector += curCoef * curVelocity.normalized; // TODO: optimize to remove magnitude computation
                totalCoef *= curCoef;
            }

        }

        externalTotalVelocity /= aCount;
        //resultVector /= totalCoef;

        if (FLAG_Debug)
        {
            Debug.DrawRay(transform.position + new Vector3(0, .5f, 0), resultVector, Color.cyan, Time.fixedDeltaTime);
            //Debug.Log(resultVector);
        }

        FlockingInfo.Alignment = resultVector;
    }
    protected List<Actor> Flocking_GetSublistOfAllowableActors
        (
        List<Actor> list
        , ActorSystem.AI.Flocking.ActorAI_FlockingParametersPreset.ActorAI_FlockingParametersPreset_FlockingParameter param
        )
    {
        List<Actor> allowedActors = new List<Actor>();

        Vector3 distance;
        float v_maxDistSquared = Mathf.Pow(param.Radius, 2);

        foreach (var a in list)
        {
            distance = a.transform.position - gameObject.transform.position;
            if (distance.sqrMagnitude <= v_maxDistSquared)
            {
                allowedActors.Add(a);
            }
        }

        return allowedActors;
    }

    #endregion


    /// <summary>
    /// This computation attempts to use the target's velocity to predict where it will be when the AI agent arrives. 
    /// </summary>
    /// <remarks>
    /// Useful for intercepting a moving target, as implied. Functionality is currently too rough for final product imo. ~Jared Freeman
    /// </remarks>
    /// <param name="t">How much contribution the new computation will have to the current value</param>
    private void UpdateInterceptComputation(float t)
    {
        if (CurrentTarget == null) return;
        if (CurrentTargetRigidbody == null) return;

        //compute a suitable intercept location
        // construct a parallel velocity vector to RB vel using this logic's standard speed.
        float dist = Info.DistanceToCurrentTargetMagnitude_AtLastPoll;
        float time =  dist / (Preset.Base.MovementSpeed - CurrentTargetRigidbody.velocity.magnitude);

        float certainty = 1 / Mathf.Log(Mathf.Clamp(dist / (Preset.Base.MovementSpeed * Time.fixedDeltaTime), 1, Mathf.Infinity));

        t = certainty;

        Info.CurrentMovementTargetIntercept_Fuzzy = (1-t) * Info.CurrentMovementTargetIntercept_Fuzzy
            + t * ((-CurrentTargetRigidbody.velocity * time));

        //if (FLAG_Debug) Debug.Log(Info.CurrentMovementTargetIntercept_Fuzzy);
    }

    #endregion

    #region State Change

    /// <summary>
    /// Fires when the AI Logic triggers a state change.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///     PLEASE use StateEnd and StateBegin unless you have a good reason for overriding this.
    ///     </para>
    ///     <para>
    ///     To override: invoke base, then hook up a switch statement and you should be good to go.
    ///     </para>
    ///     <para>
    ///     This base class fires State End and State Begin methods.
    ///     </para>
    /// </remarks>
    /// <param name="nextState">The new state to change to.</param>
    protected virtual void ChangeState(ActorAILogic_State nextState)
    {
        if (FLAG_Debug)
        {
            Debug.Log("State Change: " + nextState.ToString());
        }
        StateEnd(CurrentState);
        CurrentState = nextState;
        StateBegin(CurrentState);
    }
    /// <summary>
    /// Fires when a state ends. Useful for cleaning up temporary mutations in a given state.
    /// </summary>
    ///     <remarks>
    ///     To override: invoke base, then hook up a switch statement and you should be good to go.
    ///     </remarks>
    /// <param name="stateToEnd">The state that is ending currently.</param>
    protected virtual void StateEnd(ActorAILogic_State stateToEnd)
    {
        if(FLAG_Debug)
        {
            Debug.Log("State End: " + stateToEnd.ToString());
        }
    }
    /// <summary>
    /// Fires when a new state begins. Useful for initializing status mutations for a given state.
    /// </summary>
    ///     <remarks>
    ///     To override: invoke base, then hook up a switch statement and you should be good to go.
    ///     </remarks>
    /// <param name="stateToStart">The state that is starting.</param>
    protected virtual void StateBegin(ActorAILogic_State stateToStart)
    {
        if (FLAG_Debug)
        {
            Debug.Log("State Start: " + stateToStart.ToString());
        }
    }

    #endregion

    #region Utility Methods

    //TODO: Consider optimizing
    public bool EnemyExistsInAggroRadius()
    {
        foreach (Actor A in Singleton<ActorManager>.Instance.ActorList)
        {
            if (
                (A.gameObject.transform.position - gameObject.transform.position).sqrMagnitude <= (Preset.Base.AggroRadius * Preset.Base.AggroRadius)
                && A._Team.IsEnemy(AttachedActor._Team)
                )
            {
                if (FLAG_Debug) Debug.Log("Enemy in Aggro Range");
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Chooses a new target based on TargetPriority setting
    /// </summary>
    /// <exception cref="System.NotImplementedException">Exception thrown when no implementation exists for the supplied TargetPriority</exception>
    protected virtual void ChooseNewTarget()
    {
        //TODO: consider some more robust logic here. Could have the AI decide based on some criteria like hp remaining or threat.
        //For now proximity will do

        switch (Preset.Base._TargetPriority)
        {
            case TargetPriority.None:
                //get random from selection
                foreach (Actor A in Singleton<ActorManager>.Instance.ActorList)
                {
                    if (
                        (A.gameObject.transform.position - gameObject.transform.position).sqrMagnitude <= (Preset.Base.AggroRadius * Preset.Base.AggroRadius)
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
                        (A.gameObject.transform.position - gameObject.transform.position).sqrMagnitude <= (Preset.Base.AggroRadius * Preset.Base.AggroRadius)
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
                throw new System.NotImplementedException("No impl exits for TargetPriority: " + Preset.Base._TargetPriority.ToString());
        }
    }

    /// <summary>
    /// Per-Frame turn interpolation coroutine. Does NOT work with physics drivers!!!
    /// </summary>
    /// <returns></returns>
    protected IEnumerator I_TurnInterpolate()
    {
        bool FLAG_Done = false;
        while (!FLAG_Done)
        {
            if (FLAG_Debug) Debug.LogWarning("Obsolete Turn interp is turned on. Is this intended???");

            if (Info.CanTurn && CurrentTarget != null)
            {
                float Angle = Vector3.SignedAngle(gameObject.transform.forward, (CurrentTarget.transform.position - gameObject.transform.position).normalized, Vector3.up);

                float ScaledTurnRate = Preset.Base.TurningRate * Time.deltaTime;

                var Rot = Vector3.ProjectOnPlane((CurrentTarget.transform.position - transform.position), Vector3.up);

                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(Rot, Vector3.up), ScaledTurnRate);
            }

            yield return null;
        }
    }

    /// <summary>
    /// Rudimentary utility method that causes the Agent to grow over a period of time equal to Preset.Base.GrowDuration, then return back to original scale.
    /// </summary>
    /// <remarks>
    /// Other scale affecting methods can be disrupted by this routine!
    /// </remarks>
    protected IEnumerator I_IncreaseScale()
    {
        var DefaultScale = transform.localScale;

        float StartTime = Time.time;
        while (Time.time - StartTime < Preset.Base.GrowDuration)
        {
            transform.localScale = DefaultScale * Preset.Base.GrowCurve.Evaluate((Time.time - StartTime) / Preset.Base.GrowDuration);
            yield return null;
        }

        transform.localScale = DefaultScale;
    }

    #endregion

}
