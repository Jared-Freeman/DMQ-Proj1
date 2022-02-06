using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Intercepts NavmeshAgent's desiredVelocity and implements movement model
/// </summary>
/// 
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class AIMover : MonoBehaviour
{
    public NavMeshAgent Agent { get; protected set; }
    public Rigidbody RB;

    public Utils.Physics.CFC_MoveOptions Options;

    protected Vector3 _DesiredVelocityLastFixedUpdate_Normalized { get; private set; }
    protected Vector3 _CurDesiredVelocity { get; private set; }

    protected float _ExternalWeight { get; private set; } = .8f;
    protected Vector3 _ExternalContribution { get; private set; }
    protected Vector3 _AgentContribution { get; private set; }

    protected virtual void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        if(Agent == null)
        {
            Debug.LogError("Ref missing! Destroying this.");
        }
        RB = GetComponent<Rigidbody>();
        if (RB == null)
        {
            Debug.LogError("Ref missing! Destroying this.");
        }

        _DesiredVelocityLastFixedUpdate_Normalized = Vector3.zero;
    }

    protected virtual void Start()
    {
        Agent.updatePosition = false;
        Agent.updateRotation = false;

    }


    protected virtual void FixedUpdate()
    {
        //ignore friction contribution to external
        _ExternalContribution = new Vector3(RB.velocity.x, 0, RB.velocity.z) - _DesiredVelocityLastFixedUpdate_Normalized * Time.fixedDeltaTime;

        Debug.LogWarning(_ExternalContribution);

        if(_ExternalContribution.sqrMagnitude > 5.5f)
        {
            VelocityDecay(Vector3.zero, .1f);

            //Add a small amount of resistance force to simulate the AI trying to resist being pushed (or steering i guess?)
            // This is experimental and CAN be removed if it ends up sucking...
            RB.AddForce(.01f * (_CurDesiredVelocity - RB.velocity) * RB.mass / Time.fixedDeltaTime, ForceMode.Force);
        }
        else
        {
            VelocityDecay(Agent.desiredVelocity, Mathf.Clamp(.08f, 0, 1));
            //_CurDesiredVelocity = Agent.desiredVelocity;

            //Debug.LogWarning(Agent.desiredVelocity);


            //currently the desired velocity polling rate is lower than fixed update... Could be causing issues at lower values
            //Utils.Physics.PerformFixedContinuousMovement(ref RB, _CurDesiredVelocity, ref Options);
            RB.AddForce((_CurDesiredVelocity - RB.velocity) * RB.mass / Time.fixedDeltaTime, ForceMode.Force);
            Debug.DrawRay(transform.position, _CurDesiredVelocity * 2f, Color.green, Time.fixedDeltaTime);

        }

        //IDEA: Introduce the concept of decay into navmeshagent's thought pattern.
        //Utils.Physics.PerformFixedContinuousMovement(ref RB, Agent.desiredVelocity, ref Options);


        Agent.nextPosition = RB.position; //Update the NavmeshAgent's internal simulation
        _DesiredVelocityLastFixedUpdate_Normalized = _CurDesiredVelocity / Time.fixedDeltaTime;
    }

    void VelocityDecay(Vector3 vector, float t)
    {

        _CurDesiredVelocity = t * vector + (1 - t) * _DesiredVelocityLastFixedUpdate_Normalized * Time.fixedDeltaTime;
    }

    void OnCollisionEnter(Collision c)
    {
        Debug.DrawRay(transform.position, -c.impulse * 2f, Color.black, 2);
    }

}
