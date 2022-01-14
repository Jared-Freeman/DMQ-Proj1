using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSEventArgs
{
    public class GenericActorActionEventArgs : System.EventArgs
    {

    }
}

public class AP2_ActorAction_Base : ScriptableObject
{
    public static event System.EventHandler<CSEventArgs.GenericActorActionEventArgs> OnActionStart;

    public virtual void PerformAction(Actor Owner)
    {
        OnActionStart?.Invoke(this, new CSEventArgs.GenericActorActionEventArgs());
    }
}
