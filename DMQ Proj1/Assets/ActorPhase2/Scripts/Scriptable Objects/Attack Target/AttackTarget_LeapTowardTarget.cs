using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AP2
{
    /// <summary>
    /// Not sure if this is a good idea. Launches the owner object toward the 
    /// </summary>
    [CreateAssetMenu(fileName = "ActorAction", menuName = "ScriptableObjects/Actor Actions/Attack/Attack Target/Launch Owner Toward Target", order = 1)]
    public class AttackTarget_LeapTowardTarget : AP2_ActorAction_AttackTarget
    {
        [System.Serializable]
        public class MoreOptions
        {
            public float MaxRange = 1f;
        }

        public MoreOptions SpecialOptions;

        public override void AttackTarget(Actor Owner, GameObject Target)
        {
            base.AttackTarget(Owner, Target);

            //TODO: Consider event dispatch here
            if (Target.GetComponent<ActorStats>() != null)
            {
                throw new System.NotImplementedException("Leap Toward Target NYI");
                //Options.Target.GetComponent<ActorStats>().ApplyDamage(Options.DamageMessage);
            }
        }
    }
}

