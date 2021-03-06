using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActorSystem.AI
{
    /// <summary>
    /// Allows for multiple ActorActions to be invoked from a single dispatcher. (i.e., dealing damage and applying a physics impulse)
    /// </summary>
    //[CreateAssetMenu(fileName = "ActorAction", menuName = "ScriptableObjects/Actor Actions/Attack/Attack Target/_Set", order = 1)]
    public class AttackTarget_Set : AP2_ActorAction_AttackTarget
    {

        [System.Serializable]
        public class MoreOptions
        {
            public List<AP2_ActorAction_AttackTarget> Actions;
        }

        public MoreOptions SpecialOptions;

        public override void AttackTarget(Actor Owner, GameObject Target, Vector3 DirectionNormal = default)
        {
            base.AttackTarget(Owner, Target, DirectionNormal);

            foreach(AP2_ActorAction_AttackTarget a in SpecialOptions.Actions)
            {
                a.AttackTarget(Owner, Target, DirectionNormal);
            }
        }
    }

}