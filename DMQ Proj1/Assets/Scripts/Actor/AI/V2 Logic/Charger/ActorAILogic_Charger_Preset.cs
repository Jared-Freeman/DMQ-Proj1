using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AbilitySystem;


namespace ActorSystem.AI
{
    /// <summary>
    /// Preset for <see cref="ActorAILogic_Charger"/>
    /// </summary>
    public class ActorAILogic_Charger_Preset : ActorAI_Logic_PresetBase
    {
        /// <summary>
        /// Ability invoked when the charger collides with something during its charge state.
        /// </summary>
        public AbilitySystem.AS_Ability_Base Ability_OnChargeCollision;
    }
}