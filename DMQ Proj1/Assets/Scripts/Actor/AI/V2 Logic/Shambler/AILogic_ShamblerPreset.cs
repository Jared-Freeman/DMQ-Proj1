using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AbilitySystem;

namespace ActorSystem.AI
{
    /// <summary>
    /// Preset for the Shambler AI Logic. A Shambler logic will simply approach an enemy and try to attack them using their attached Ability when within range.
    /// </summary>
    [CreateAssetMenu(fileName = "AI_", menuName = "Actor/AI Logic/Shambler", order = 2)]
    public class AILogic_ShamblerPreset : ActorAI_Logic_PresetBase
    {
        public AIOptions_ShamblerPreset Shambler_Options;

        //inspector helper
        [System.Serializable]
        public class AIOptions_ShamblerPreset
        {
            public AS_Ability_Base AttackAbility;

            public float AttackPrepareDistance = 2.25f;
            public float AttackLoseDistance = 3.5f;

            [Min(0f)]
            public float AttackPause = .5f;
            public float GrowDuration = .5f;
            public AnimationCurve GrowCurve;
        }
    }
}