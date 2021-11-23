using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ActorAI : Actor
{
    #region members

    NavMeshAgent NavAgent;
    ActorAI_Logic Logic;

    #endregion

    new protected void Start()
    {
        base.Start();
        NavAgent = GetComponent<NavMeshAgent>();
        if (NavAgent == null) Debug.LogError("Navmesh Agent not discovered!");
    }

    new protected void Update()
    {
        base.Update();

    }
}
