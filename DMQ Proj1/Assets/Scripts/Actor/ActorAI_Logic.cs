using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
//This class holds the reigns of any AI. Its goal is to take the ActorAI data structure and drive it during game simulation.
[RequireComponent(typeof(ActorAI))]
[RequireComponent(typeof(NavMeshAgent))]
public class ActorAI_Logic : MonoBehaviour
{
    #region members

    public ActorAI AttachedActor;
    public NavMeshAgent NavAgent;
    public Animator animator;
    #endregion

    protected virtual void Awake()
    {
        AttachedActor = GetComponent<ActorAI>();
        NavAgent = GetComponent<NavMeshAgent>();
        if (NavAgent == null) Debug.LogError("Navmesh Agent not discovered!");
    }

    protected virtual void Start()
    {
    }

    public void UpdateLogic()
    {
        
    }
}
