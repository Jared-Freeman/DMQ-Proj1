using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorAction_DealDamageGeneric : ActorAction
{
    #region Members
    #endregion

    public static event System.EventHandler<MonobehaviourEventArgs> Action_GenericDamageEvent;

    // Start is called before the first frame update
    new protected void Start()
    {
        base.Start();
    }
    
    #region Methods
    new public void OnActionStart()
    {
        Action_GenericDamageEvent.Invoke(this, new MonobehaviourEventArgs(this));
    }
    new public void ActionUpdate()
    {

    }
    new public void OnActionEnd()
    {

    }
    #endregion
}
