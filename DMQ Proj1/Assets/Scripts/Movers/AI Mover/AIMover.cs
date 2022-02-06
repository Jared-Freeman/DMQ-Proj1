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
        VelocityDecay();

        //Debug.LogWarning(Agent.desiredVelocity);

        //currently the desired velocity polling rate is lower than fixed update... Could be causing issues at lower values
        Utils.Physics.PerformFixedContinuousMovement(ref RB, _CurDesiredVelocity, ref Options);

        //IDEA: Introduce the concept of decay into navmeshagent's thought pattern.
        //Utils.Physics.PerformFixedContinuousMovement(ref RB, Agent.desiredVelocity, ref Options);


        Agent.nextPosition = RB.position; //Update the NavmeshAgent's internal simulation
        _DesiredVelocityLastFixedUpdate_Normalized = _CurDesiredVelocity / Time.fixedDeltaTime;
    }

    void VelocityDecay()
    {
        float t = .4f;

        _CurDesiredVelocity = t * Agent.desiredVelocity + (1 - t) * _DesiredVelocityLastFixedUpdate_Normalized * Time.fixedDeltaTime;
    }
}
