using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

            [Min(0f)]
            public float AttackRange = 1.25f;

            public float MovementSpeed = 2f; //TODO: Currently not hooked up to navmesh agent
            public float StopSlideDistance = .5f;

            public float AggroRadius = 20;

            [Tooltip("Max angle the agent can move toward without needing to stop and turn")]
            public float MaxFacingAngle = 250f;

            [Min(0f)]
            public float LungePrepareDistance = 2f;
            public float LungeLosePrepareDistance = 3f;
            public float LungeDistance = 3f;
            [Min(0f)]
            public float LungePause = 2f;
            [Min(0f)]
            public float LungeSpeed = 8f;
            [Min(0f)]
            public float LungeTimeout = 1.25f;

            public List<ImpactFX.ImpactEffect> Lunge_ImpactEffects = new List<ImpactFX.ImpactEffect>();

            [Tooltip("Deg/sec")]
            public float TurningRate = 360f;

            public AnimationCurve GrowCurve;
            public float GrowDuration = .5f;

            public ActorAI_Logic.TargetPriority _TargetPriority = ActorAI_Logic.TargetPriority.Proximity;
        }
    }


    [CreateAssetMenu(fileName = "AI_", menuName = "Actor/AI Logic/Generic Charging Enemy", order = 2)]
    public class AP2_GenericEnemyAI_LogicPreset : ActorAI_Logic_PresetBase
    {
        AIOptions_AP2_GenericEnemyAI GEAI_Options;

        //inspector helper
        [System.Serializable]
        public class AIOptions_AP2_GenericEnemyAI
        {
            public bool DrawDebugGizmos = false;

            [Min(0f)]
            public float AttackRange = 1.25f;

            public float MovementSpeed = 2f; //TODO: Currently not hooked up to navmesh agent
            public float StopSlideDistance = .5f;

            public float AggroRadius = 20;

            [Tooltip("Max angle the agent can move toward without needing to stop and turn")]
            public float MaxFacingAngle = 250f;

            [Min(0f)]
            public float LungePrepareDistance = 2f;
            public float LungeLosePrepareDistance = 3f;
            public float LungeDistance = 3f;
            [Min(0f)]
            public float LungePause = 2f;
            [Min(0f)]
            public float LungeSpeed = 8f;
            [Min(0f)]
            public float LungeTimeout = 1.25f;

            public List<ImpactFX.ImpactEffect> Lunge_ImpactEffects = new List<ImpactFX.ImpactEffect>();

            public AnimationCurve GrowCurve;
            public float GrowDuration = .5f;
        }
    }
}