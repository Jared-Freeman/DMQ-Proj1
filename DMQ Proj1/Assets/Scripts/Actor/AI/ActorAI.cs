using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class ActorAI : Actor
{
    #region members
    ActorAI_Logic Logic;
    #endregion

    new protected void Start()
    {
        base.Start();
        Logic = GetComponent<ActorAI_Logic>();
    }

    new protected void Update()
    {
        base.Update();
    }
}
