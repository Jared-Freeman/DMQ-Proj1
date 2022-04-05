using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AbilitySystem;

namespace ActorSystem.AI
{
    [CreateAssetMenu(fileName = "AI_", menuName = "Actor/AI Logic/[DEPRECATED] Charger", order = 2)]
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

            //public List<ImpactFX.ImpactEffect> Lunge_ImpactEffects = new List<ImpactFX.ImpactEffect>();
            public AS_Ability_Base LungeCollisionAbility;


            [Min(0f)]
            public float LungePause = .5f;
            public float GrowDuration = .5f;
            public AnimationCurve GrowCurve;
        }
    }
}