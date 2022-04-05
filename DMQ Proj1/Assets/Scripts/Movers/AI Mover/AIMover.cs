using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using ActorSystem.AI;


public class AIMoverEventArgs : System.EventArgs
{
    /// <summary>
    /// AI Move messaging packet.
    /// </summary>
    /// <param name="vel">velocity</param>
    /// <param name="act">actor ref</param>
    public AIMoverEventArgs(Vector3 vel, Actor act)
    {
        velocity = vel;
        actor = act;
    }
    public Vector3 velocity;
    public Actor actor;
}


/// <summary>
/// Intercepts NavmeshAgent's desiredVelocity and implements movement model
/// </summary>
/// 
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(ActorAI_Logic))]
public class AIMover : MonoBehaviour
{
    [Min(0f)]
    [Header("If you dont know what this is, don't mess with it!")]
    public float VelocityDecay_tParam = .08f;

    public bool FLAG_DEBUG = false;

    public NavMeshAgent Agent { get; protected set; }
    public Rigidbody RB { get; protected set; }
    public ActorAI_Logic Logic { get; protected set; }

    public Actor attachedActor { get; protected set; }

    protected Vector3 _DesiredVelocityLastFixedUpdate_Normalized { get; private set; }
    protected Vector3 _CurDesiredVelocity { get; private set; }

    protected Vector3 _ExternalContribution { get; private set; }


    //Events
    public static event System.EventHandler<AIMoverEventArgs> OnVelocityUpdate;


    protected virtual void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        if(Agent == null)
        {
            Debug.LogError("Ref missing! Destroying this.");
            Destroy(gameObject);
        }
        RB = GetComponent<Rigidbody>();
        if (RB == null)
        {
            Debug.LogError("Ref missing! Destroying this.");
            Destroy(gameObject);
        }
        Logic = GetComponent<ActorAI_Logic>();
        if(Logic == null)
        {
            Debug.LogError("Ref missing! Destroying this.");
            Destroy(gameObject);
        }

        _DesiredVelocityLastFixedUpdate_Normalized = Vector3.zero;


        attachedActor = GetComponent<Actor>();
        if (!attachedActor)
            Debug.Log("No attached actor.");
    }

    protected virtual void Start()
    {
        Agent.updatePosition = false;
        Agent.updateRotation = false;

    }


    protected virtual void FixedUpdate()
    {
        if (!Logic.CanMove)
        {
            Agent.nextPosition = RB.position; //Update the NavmeshAgent's internal simulation
            return;
        }

        //ignore friction contribution to external
        _ExternalContribution = new Vector3(RB.velocity.x, 0, RB.velocity.z) - _DesiredVelocityLastFixedUpdate_Normalized * Time.fixedDeltaTime;

        //Debug.LogWarning(_ExternalContribution);

        if(_ExternalContribution.sqrMagnitude > 5.5f)
        {
            VelocityDecay(Vector3.zero, .1f);

            //Add a small amount of resistance force to simulate the AI trying to resist being pushed (or steering i guess?)
            // This is experimental and CAN be removed if it ends up sucking...
            RB.AddForce(.01f * (_CurDesiredVelocity - RB.velocity) * RB.mass / Time.fixedDeltaTime, ForceMode.Force);
        }
        else
        {
            VelocityDecay(Logic.DesiredVelocity, Mathf.Clamp(.08f, 0, 1));
            //_CurDesiredVelocity = Agent.desiredVelocity;

            //Debug.LogWarning(Agent.desiredVelocity);


            //currently the desired velocity polling rate is lower than fixed update... Could be causing issues at lower values
            //Utils.Physics.PerformFixedContinuousMovement(ref RB, _CurDesiredVelocity, ref Options);
            RB.AddForce((_CurDesiredVelocity - RB.velocity) * RB.mass / Time.fixedDeltaTime, ForceMode.Force);

#if UNITY_EDITOR
            if(FLAG_DEBUG) Debug.DrawRay(transform.position, _CurDesiredVelocity, Color.green, Time.fixedDeltaTime);
#endif

        }

        //IDEA: Introduce the concept of decay into navmeshagent's thought pattern.
        //Utils.Physics.PerformFixedContinuousMovement(ref RB, Agent.desiredVelocity, ref Options);


        Agent.nextPosition = RB.position; //Update the NavmeshAgent's internal simulation
        _DesiredVelocityLastFixedUpdate_Normalized = _CurDesiredVelocity / Time.fixedDeltaTime;

        //Dispatch event to AI animator proxy
        //OnVelocityUpdate?.Invoke(this, new AIMoverEventArgs(RB.velocity.magnitude, attachedActor));
        OnVelocityUpdate?.Invoke(this, new AIMoverEventArgs(RB.velocity, attachedActor));
    }

    void VelocityDecay(Vector3 vector, float t)
    {

        _CurDesiredVelocity = t * vector + (1 - t) * _DesiredVelocityLastFixedUpdate_Normalized * Time.fixedDeltaTime;
    }
}
