using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ActorAction", menuName = "ScriptableObjects/Actor Actions/Attack/Attack Target", order = 1)]
public class AP2_ActorAction_AttackTarget : AP2_ActorAction_Base
{
    [System.Serializable]
    public struct ScriptOptions
    {
        public GameObject Target;
        public AP2_DamageMessage DamageMessage;
    }

    public ScriptOptions Options;

    public override void PerformAction(Actor Owner)
    {
        base.PerformAction(Owner);

        //TODO: Consider event dispatch here

        //This base behavior simply applies damage immediately
        if(Options.Target.GetComponent<ActorStats>() != null)
        {
            Options.Target.GetComponent<ActorStats>().ApplyDamage(Options.DamageMessage);
        }
    }
}
