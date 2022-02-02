using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AbilitySystem
{
    /// <summary>
    /// The component part of an ability.
    /// Abilities are constructed using a preset for the settings data, and a component instance that holds the state data and a reference to the preset
    /// </summary>
    public abstract class AS_Ability_Instance_Base : MonoBehaviour
    {
        /// <summary>
        /// Override this to enforce proper subclassing. I make an accessible member in subclass presets and edit the get/set to link to that.
        /// See my implementations if you have questions.
        /// </summary>
        public virtual AS_Ability_Base Settings { get; set; }

        #region Members


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

        public virtual void Awake()
        {
            if (Settings == null)
            {
                Debug.LogError("Ability missing preset! Destroying");
                Destroy(this);
            }

            Cooldown = new Utils.CooldownTracker(Settings.Cooldown);
        }

        /// <summary>
        /// Virtual function handles cooldown consumption. Can listen to this in child impls to evade duplicate logic
        /// </summary>
        /// <returns>true if ability executed successfully</returns>
        public virtual bool ExecuteAbility()
        {
            if(Cooldown.ConsumeCooldown())
                return true;
            return false;
        }

        #endregion
    }
}