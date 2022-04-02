using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class ActorAI : Actor
{
    #region members

    ActorAI_Logic Logic;

    #endregion

    protected override void Awake()
    {
        base.Awake();
        Logic = GetComponent<ActorAI_Logic>();
        if (!Utils.Testing.ReferenceIsValid(Logic)) Destroy(gameObject);
    }
}
