using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AP2_RangedAI_1 : ActorAI_Logic
{

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

    //protected override void Start()
    //{
    //    base.Start();

    //    ChangeState(State.Idle);
    //    StartCoroutine(UpdateAI());
    //    StartCoroutine(I_TurnInterpolate());
    //}
    #endregion

}
