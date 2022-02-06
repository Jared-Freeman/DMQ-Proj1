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
    }

    protected virtual void Start()
    {
        Agent.updatePosition = false;
        Agent.updateRotation = false;
    }


    protected virtual void FixedUpdate()
    {
        //Debug.LogWarning(Agent.desiredVelocity);

        Utils.Physics.PerformFixedContinuousMovement(ref RB, Agent.desiredVelocity, ref Options);
        Agent.nextPosition = RB.position;
    }
}
