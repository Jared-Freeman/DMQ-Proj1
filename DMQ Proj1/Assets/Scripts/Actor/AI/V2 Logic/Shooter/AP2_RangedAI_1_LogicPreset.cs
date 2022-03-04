using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActorSystem.AI
{
    [CreateAssetMenu(fileName = "AI_", menuName = "Actor/AI Logic/Shooter", order = 2)]
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
