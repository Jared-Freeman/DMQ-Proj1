using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AbilitySystem;
using EffectTree;
using System;

/// <summary>
/// Instead of needing this component to be swapped, it can instead have its Preset data changed
/// </summary>
namespace ActorSystem.Player
{
    public class ActorPlayer_Skill_Instance : MonoBehaviour
    {
        #region Members

        [SerializeField] protected ActorPlayer_Skill_Preset _Preset; //We want to change the preset during runtime through a Property

        public Actor Owner { get; protected set; }

        #region Properties

        public ActorPlayer_Skill_Preset Preset 
        { 
            get { return _Preset; }
            set 
            {
                _Preset = value;
                ChangeStateData(value);
            } 
        }
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

        #endregion

        #endregion

        /// <summary>
        /// Change the state data to match a newly supplied skill preset
        /// </summary>
        /// <param name="value">new Preset</param>
        private void ChangeStateData(ActorPlayer_Skill_Preset value)
        {
            //Destroy previous ability instance
            Destroy(Ability);
            Ability = value.Ability.GetInstance(gameObject);
        }

        /// <summary>
        /// Interface for player to cast this skill. Hook this function up to a player IO agent of some kind.
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
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