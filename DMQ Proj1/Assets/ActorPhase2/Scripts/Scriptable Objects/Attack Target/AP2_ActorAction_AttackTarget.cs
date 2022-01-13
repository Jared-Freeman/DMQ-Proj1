﻿using System.Collections;
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
            public GameObject Target;
            public AP2_DamageMessage DamageMessage;
        }

        public ScriptOptions Options;

        public override void PerformAction(Actor Owner)
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
        public override void PerformAction(Actor Owner)
        {
            base.PerformAction(Owner);

            //TODO: Consider event dispatch here

            //This base behavior simply applies damage immediately
            if (Options.Target.GetComponent<ActorStats>() != null)
            {
                Options.Target.GetComponent<ActorStats>().ApplyDamage(Options.DamageMessage);
            }
        }
    }
}