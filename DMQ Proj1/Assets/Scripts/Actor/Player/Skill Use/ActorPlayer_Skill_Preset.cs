using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ActorSystem.Player
{
    /// <summary>
    /// Skills are Abilities with an energy cost. They are linked to player IO typically
    /// </summary>
    [CreateAssetMenu(fileName = "SKILL_", menuName = "Actor/Player/Skill", order = 2)]
    public class ActorPlayer_Skill_Preset : ScriptableObject
    {
        public AbilitySystem.AS_Ability_Base Ability;
        public float EnergyCost = 0f;

        /// <summary>
        /// Instantiates a skill instance and attaches it to <paramref name="componentOwner"/>.
        /// Please note that skill instances can be reused by pointing their property to a new Preset SO
        /// </summary>
        /// <param name="componentOwner"></param>
        /// <returns></returns>
        public ActorPlayer_Skill_Instance CreateInstance(GameObject componentOwner)
        {
            if (componentOwner == null) return null;

            var i = componentOwner.AddComponent<ActorPlayer_Skill_Instance>();
            i.Ability = Ability.GetInstance(componentOwner);

            //failure if ability instance fails
            if (i.Ability == null) { Destroy(i); return null; }

            return i;
        }
    }
}