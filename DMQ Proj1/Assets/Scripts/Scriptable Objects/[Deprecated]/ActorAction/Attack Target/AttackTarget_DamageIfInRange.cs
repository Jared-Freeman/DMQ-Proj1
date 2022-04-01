using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActorSystem.AI
{
    /// <summary>
    /// Applies damage to Target if within range
    /// </summary>
    //[CreateAssetMenu(fileName = "ActorAction", menuName = "ScriptableObjects/Actor Actions/Attack/Attack Target/Damage Target if in Range", order = 1)]
    public class AttackTarget_DamageIfInRange : AP2_ActorAction_AttackTarget
    {
        [System.Serializable]
        public class MoreOptions
        {
            [Min(0f)]
            public float MaxRange = 1f;
        }

        public MoreOptions SpecialOptions;

        public override void AttackTarget(Actor Owner, GameObject Target, Vector3 DirectionNormal = default)
        {
            base.AttackTarget(Owner, Target, DirectionNormal);

            //TODO: Consider event dispatch here

            //This base behavior simply applies damage immediately if within specified range
            if (
                Target.GetComponent<ActorStats>() != null 
                && (Owner.gameObject.transform.position - Target.transform.position).sqrMagnitude <= SpecialOptions.MaxRange * SpecialOptions.MaxRange
                )
            {
                if (FLAG_Debug) Debug.Log("In range. Sending Damage Message");
                Target.GetComponent<ActorStats>().ApplyDamage(Options.DamageMessage.CreateMessage(Owner.gameObject, Owner._Team, Vector3.zero, Owner.gameObject, Vector3.zero));
            }
            else if (FLAG_Debug) Debug.Log("null stats, or not in range");
        }
    }
}
