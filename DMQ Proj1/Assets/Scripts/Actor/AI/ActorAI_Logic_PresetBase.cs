using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AbilitySystem;

namespace ActorSystem.AI
{
    [CreateAssetMenu(fileName = "AI_", menuName = "Actor/AI Logic/Base", order = 1)]
    public class ActorAI_Logic_PresetBase : ScriptableObject
    {
        public AIOptions Base;

        //inspector helper
        [System.Serializable]
        public class AIOptions
        {
            public bool DrawDebugGizmos = false;

            /// <summary>
            /// AI Predicts where the CurrentTarget is going to be, and tries to move there
            /// </summary>
            public bool InterceptCurrentTarget = true;

            /// <summary>
            /// Can this AI invoke attacks while moving?
            /// </summary>
            public bool AttackWhileMoving = false;
            /// <summary>
            /// Velocity Tolerance that counts as stationary
            /// </summary>
            public float StationaryVelocityThreshold = .05f;

            [Min(0f)]
            public float AttackRange = 1.25f;

            public float MovementSpeed = 2f; //TODO: Currently not hooked up to navmesh agent
            public float StopSlideDistance = .5f;

            public float AggroRadius = 20;

            [Tooltip("Max angle the agent can move toward without needing to stop and turn")]
            public float MaxFacingAngle = 60;
            [Tooltip("Deg/sec")]
            public float TurningRate = 240;

            public AnimationCurve GrowCurve;
            public float GrowDuration = 1;

            public ActorAI_Logic.TargetPriority _TargetPriority = ActorAI_Logic.TargetPriority.Proximity;
        }
    }
}