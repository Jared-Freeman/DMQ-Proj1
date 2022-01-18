using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AP2
{
    /// <summary>
    /// Abstract impl of AttackTarget
    /// </summary>
    public class AP2_ActorAction_AttackTarget : AP2_ActorAction_Base
    {
        public bool FLAG_Debug = false;

        [System.Serializable]
        public struct ScriptOptions
        {
            public AP2_DamageMessagePreset DamageMessage;
        }

        public ScriptOptions Options;

        public virtual void AttackTarget(Actor Owner, GameObject Target, Vector3 DirectionNormal = default)
        {
            if (FLAG_Debug) Debug.Log("PerformAction()");

            base.PerformAction(Owner);
        }
    }

    /// <summary>
    /// Generic Impl of this Action
    /// </summary>
    [CreateAssetMenu(fileName = "ActorAction", menuName = "ScriptableObjects/Actor Actions/Attack/Attack Target/Generic Attack", order = 1)]
    public class AttackTarget_Generic : AP2_ActorAction_AttackTarget
    {
        public override void AttackTarget(Actor Owner, GameObject Target, Vector3 DirectionNormal = default)
        {
            base.AttackTarget(Owner, Target, DirectionNormal);

            //TODO: Consider event dispatch here

            //This base behavior simply applies damage immediately
            if (Target.GetComponent<ActorStats>() != null)
            {
                Target.GetComponent<ActorStats>().ApplyDamage(Options.DamageMessage.CreateMessage(Owner.gameObject, Owner._Team, Vector3.zero, Owner.gameObject, Vector3.zero));
            }
        }
    }
}
