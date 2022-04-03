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


            #region Intercept
            [Space(7)]
            
            /// <summary>
            /// AI Predicts where the CurrentTarget is going to be, and tries to move there
            /// </summary>
            public bool InterceptCurrentTarget = true;
            /// <summary>
            /// How much of the intercept prediction should be considered when leading the target. 
            /// </summary>
            /// <remarks>
            /// Helps to account for uncertainty without a huge dev cost, but also can modify AI behavior further, making it unique!
            /// </remarks>
            [Range(0, 1)]
            public float InterceptCurrentTargetStrength = .25f;
            /// <summary>
            /// The distance where InterceptCurrentTarget is no longer considered and the target is no longer "lead"
            /// </summary>
            public float InterceptCurrentTargetDisableDistance = 6f;

            [Space(7)]
            #endregion

            /// <summary>
            /// Is the agent allowed to cancel its charging phase when its target leaves range? 
            /// </summary>
            /// <remarks>
            /// This seems best to leave turned to false in most circumstances. When in doubt set to false.
            /// </remarks>
            public bool CanCancelAttackEarly = false;
            /// <summary>
            /// Can this AI invoke attacks while moving? Partially NYI afaik... -Jared 4/1/2022
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

            //should these still be here???
            public AnimationCurve GrowCurve;
            public float GrowDuration = 1;

            public ActorAI_Logic.TargetPriority _TargetPriority = ActorAI_Logic.TargetPriority.Proximity;
        }
    }
}