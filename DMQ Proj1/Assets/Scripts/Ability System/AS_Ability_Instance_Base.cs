using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem.EventArgs
{
    public class AS_Ability_Instance_Base_EventArgs : System.EventArgs
    {
        public AbilitySystem.AS_Ability_Instance_Base TriggeringInstance;

        public AS_Ability_Instance_Base_EventArgs(AbilitySystem.AS_Ability_Instance_Base triggeringInstance)
        {
            TriggeringInstance = triggeringInstance;
        }
    }
}

namespace AbilitySystem
{
    

    /// <summary>
    /// The component part of an ability.
    /// Abilities are constructed using a preset for the settings data, and a component instance that holds the state data and a reference to the preset
    /// </summary>
    public class AS_Ability_Instance_Base : MonoBehaviour
    {
        #region Events

        public static event System.EventHandler<AbilitySystem.EventArgs.AS_Ability_Instance_Base_EventArgs> OnAbilityUsed;
        /// <summary>
        /// Fires when cooldown is available for this ability instance.
        /// </summary>
        public static event System.EventHandler<AbilitySystem.EventArgs.AS_Ability_Instance_Base_EventArgs> OnAbilityAvailable;

        #endregion

        #region Members
        /// <summary>
        /// Override this to enforce proper subclassing. I make an accessible member in subclass presets and edit the get/set to link to that.
        /// See my implementations if you have questions.
        /// </summary>
        public virtual AS_Ability_Base Settings { get; set; }

        //refs
        public Actor Owner;
        public Utils.CooldownTracker Cooldown { get; protected set; }

        //properties
        public bool AbilityCurrentlyActive { get; protected set; }

        public bool CanCastAbility
        {
            get
            {
                if (Cooldown.CooldownAvailable) return true;
                return false;
            }
        }

        #endregion

        #region Virtual Methods

        public virtual void Start()
        {
            if (Settings == null)
            {
                Debug.LogError("Ability missing preset! Destroying");
                Destroy(this);
            }

            Cooldown = new Utils.CooldownTracker(Settings.Cooldown);

            Cooldown.OnCooldownUsed += Cooldown_OnCooldownUsed;
            Cooldown.OnCooldownAvailable += Cooldown_OnCooldownAvailable;
        }
        /// <summary>
        /// Virtual function handles cooldown consumption. Can listen to this in child impls to evade duplicate logic
        /// </summary>
        /// <returns>true if ability executed successfully</returns>
        public virtual bool ExecuteAbility(ref EffectTree.EffectContext ctx)
        {
            if(Settings.Conditions.Evaluate(ref ctx) && Cooldown.ConsumeCooldown())
            {
                Settings.Effect?.Invoke(ref ctx);
                OnAbilityUsed?.Invoke(this, new EventArgs.AS_Ability_Instance_Base_EventArgs(this));
                return true;
            }
            else if(!Cooldown.CooldownAvailable)
            {
                Settings.EffectEvents.Effect_CastWhileOnCooldown.Invoke(ref ctx);
            }
            return false;
        }

        #endregion

        #region Event Handlers

        private void Cooldown_OnCooldownAvailable(object sender, Utils.CooldownTracker.CooldownTrackerEventArgs e)
        {
            Debug.Log("Cooldown Avaialbel!!!");

            //note this is a very limited effect context
            EffectTree.EffectContext ctx = Owner.GetDefaultEffectContext();
            Settings.EffectEvents.Effect_CooldownAvailable?.Invoke(ref ctx);

            OnAbilityAvailable?.Invoke(this, new EventArgs.AS_Ability_Instance_Base_EventArgs(this));
        }

        private void Cooldown_OnCooldownUsed(object sender, Utils.CooldownTracker.CooldownTrackerEventArgs e)
        {

        }

        #endregion

        void OnDestroy()
        {
            if (Cooldown != null)
            {
                Cooldown.OnCooldownUsed -= Cooldown_OnCooldownUsed;
                Cooldown.OnCooldownAvailable -= Cooldown_OnCooldownAvailable;
            }
        }

    }
}