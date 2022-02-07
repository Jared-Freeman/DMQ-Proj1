using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AbilitySystem;
using EffectTree;

namespace ActorSystem.Player
{
    public class ActorPlayer_Skill_Instance : MonoBehaviour
    {
        public ActorPlayer_Skill_Preset Preset;
        public Actor Owner;

        public AS_Ability_Instance_Base Ability { get; set; } //Properties are hidden in the inspector during runtime

        protected virtual void Awake()
        {
            Owner = GetComponent<Actor>();
            if(Owner == null)
            {
                Debug.LogError("Ref not found! Destroying.");
                Destroy(this);
            }
        }

        public virtual bool CastSkill(ref EffectContext ctx)
        {
            if(ctx != null && Owner.Stats.EnergyCurrent >= Preset.EnergyCost)
            {
                Owner.Stats.ConsumeEnergy(Preset.EnergyCost);
                return Ability.ExecuteAbility(ref ctx);
            }
            return false;
        }
    }
}