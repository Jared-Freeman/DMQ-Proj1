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


    [CreateAssetMenu(fileName = "AI_", menuName = "Actor/AI Logic/Generic Charging Enemy", order = 2)]
    public class AP2_GenericEnemyAI_LogicPreset : ActorAI_Logic_PresetBase
    {
        //TODO: What goes here versus the base preset?
        public AIOptions_AP2_GenericEnemyAI GEAI_Options;

        //inspector helper
        [System.Serializable]
        public class AIOptions_AP2_GenericEnemyAI
        {
            [Min(0f)]
            public float LungePrepareDistance = 4;
            public float LungeLosePrepareDistance = 5.5f;
            public float LungeDistance = 4.5f;
            [Min(0f)]
            public float LungeSpeed = 22f;
            [Min(0f)]
            public float LungeTimeout = 1.25f;

            public List<ImpactFX.ImpactEffect> Lunge_ImpactEffects = new List<ImpactFX.ImpactEffect>();


            [Min(0f)]
            public float LungePause = .5f;
            public float GrowDuration = .5f;
            public AnimationCurve GrowCurve;
        }
    }


    [CreateAssetMenu(fileName = "AI_", menuName = "Actor/AI Logic/Generic Ranged Enemy", order = 2)]
    public class AP2_RangedAI_1_LogicPreset : ActorAI_Logic_PresetBase
    {
        //TODO: What goes here versus the base preset?
        public AIOptions_AP2_RangedAI_1_LogicPreset RAI_Options;

        //inspector helper
        [System.Serializable]
        public class AIOptions_AP2_RangedAI_1_LogicPreset
        {
            public float AttackCooldown;
            public float AttackRangeMin;
            public float AttackRangeMax;
            public float AttackChargeTime;

            public AbilitySystem.AS_Ability_Base AttackAbility;
        }
    }
}