using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AbilitySystem
{
    /// <summary>
    /// Preset for use with <see cref="AS_ExecuteAbilityOnPhysicCollision"/>.
    /// </summary>
    [CreateAssetMenu(fileName = "AS_Invoker_", menuName = "Ability/Invokers/Physic Collision Preset", order = 2)]
    public class AS_ExecuteAbilityOnPhysicCollisionPreset : ScriptableObject
    {
        /// <summary>
        /// Ability to invoke upon CollisionEnter Unity message
        /// </summary>
        public AbilitySystem.AS_Ability_Base Ability_CollisionEnter;
        /// <summary>
        /// Ability to invoke upon CollisionStay Unity message
        /// </summary>
        /// <remarks>
        /// Recall that abilities have a cooldown. Using a short cooldown, CollisionStay can be useful for having sparks fly when we're scraping against metal
        /// </remarks>
        public AbilitySystem.AS_Ability_Base Ability_CollisionStay;
        /// <summary>
        /// Ability to invoke upon CollisionExit Unity message
        /// </summary>
        public AbilitySystem.AS_Ability_Base Ability_CollisionExit;
    }
}