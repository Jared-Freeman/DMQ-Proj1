using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    /// <summary>
    /// A simple tool to load a cooldown from an ability instance IN THE INSPECTOR into the displayer module during runtime.
    /// See Utils.CooldownDisplayer
    /// </summary>
    [RequireComponent(typeof(CooldownDisplayer))]
    public class CooldownDisplay_AbilityProxy : MonoBehaviour
    {
        public AbilitySystem.AS_Ability_Instance_Base Ability;
        private AbilitySystem.AS_Ability_Instance_Base buffer_Ability;
        private CooldownDisplayer CD;

        private void Awake()
        {
            CD = GetComponent<CooldownDisplayer>();
        }

        private void Update()
        {
            //check for a ref update
            if(buffer_Ability != Ability)
            {
                buffer_Ability = Ability;
                CD.Listener.Cooldown = Ability.Cooldown;
            }
        }

    }
}