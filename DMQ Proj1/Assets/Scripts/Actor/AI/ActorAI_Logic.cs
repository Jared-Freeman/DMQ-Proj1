using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using ActorSystem.AI;

//This class holds the reigns of any AI. Its goal is to take the ActorAI data structure and drive it during game simulation.
[RequireComponent(typeof(ActorAI))]
[RequireComponent(typeof(NavMeshAgent))]
public class ActorAI_Logic : MonoBehaviour
{
    #region members

    public enum TargetPriority { None, Proximity }

    //For Logic subroutines (minimize cost-per-frame)
    protected readonly static float _RoutineSleepDuration = .125f; //8 times / sec

    //flags
    public bool FLAG_Debug = false;

    public AILogicOptions BaseOptions = new AILogicOptions();


    protected StateInfo Info = new StateInfo();

    [SerializeField]
    private ActorAI_Logic_PresetBase _Preset;
    public virtual ActorAI_Logic_PresetBase Preset
    {
        get { return _Preset; }
        set { _Preset = value; }
    }


    public GameObject CurrentTarget { get { return Info.CurrentTarget; } protected set { Info.CurrentTarget = value; } }
    public ActorAI AttachedActor { get; protected set; }
    public NavMeshAgent NavAgent { get; protected set; }
    public Animator Animator { get; protected set; }
    #endregion

    #region Helper data

    //inspector helper
    [System.Serializable]
    public class AILogicOptions
    {
        [Header("Aggro")]
        public float AggroRadius = 20;


        [Header("Moving")]
        public float MovementSpeed = 2f; //TODO: Currently not hooked up to navmesh agent
        public float StopSlideDistance = .5f;
        

        [Header("Turning")]
        [Tooltip("Max angle the agent can move toward without needing to stop and turn")]
        public float MaxFacingAngle = 250f;
        [Tooltip("Deg/sec")]
        public float TurningRate = 360f;
    }

    //internal helper
    protected class StateInfo
    {
        public List<Coroutine> ActiveRoutines = new List<Coroutine>();

        public float LungeStartTime = 0f;
        public bool CanTurn = false;
        public int CurrentAttacksInvoked = 0;

        public GameObject CurrentTarget;
    }

    #endregion

    protected virtual void Awake()
    {
        AttachedActor = GetComponent<ActorAI>();
        NavAgent = GetComponent<NavMeshAgent>();
        if (NavAgent == null) Debug.LogError("Navmesh Agent not discovered!");

        if (Preset == null) Debug.LogError("Preset is null ref! Is it the correct SO subclass?");


        //Overrides of NavAgent properties
        NavAgent.updateRotation = false;
    }

    protected virtual void Start()
    {
        //only do per-frame interp if we DONT have a rigidbody active, or it's kinematic
        if (gameObject.GetComponent<Rigidbody>() == null) StartCoroutine(I_TurnInterpolate());
        else if (gameObject.GetComponent<Rigidbody>()?.isKinematic == true) StartCoroutine(I_TurnInterpolate());
    }

    /// <summary>
    /// Rigidbody compliant turn interpolation... This solves stutter issue from the coroutine method
    /// </summary>
    void FixedUpdate()
    {
        var RB = gameObject.GetComponent<Rigidbody>();

        if (RB != null && RB.isKinematic == false && Info.CanTurn && CurrentTarget != null)
        {
            float Angle = Vector3.SignedAngle(gameObject.transform.forward, (CurrentTarget.transform.position - gameObject.transform.position).normalized, Vector3.up);

            float ScaledTurnRate = Preset.Base.TurningRate * Time.fixedDeltaTime;

            var Rot = Vector3.ProjectOnPlane((CurrentTarget.transform.position - transform.position), Vector3.up);
            var QResult = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(Rot, Vector3.up), ScaledTurnRate);

            //transform.rotation = QResult;

            RB.MoveRotation(QResult);
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
}
