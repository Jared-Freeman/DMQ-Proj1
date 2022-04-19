using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AbilitySystem
{
    /// <summary>
    /// Attempts to cast the supplied ability when a trigger accesses this Component
    /// </summary>
    public class AS_ExecuteAbilityOnScriptTrigger : MonoBehaviour
    {
        public AbilitySystem.AS_Ability_Base Ability;
        public AbilitySystem.AS_Ability_Instance_Base AbilityInstance { get => _AbilityInstance; }

        //protected members
        protected AbilitySystem.AS_Ability_Instance_Base _AbilityInstance;


        // Start is called before the first frame update
        void Start()
        {
            _AbilityInstance = Ability.GetInstance(gameObject);
        }

        /// <summary>
        /// Public facing method to invoke the attached ability
        /// </summary>
        /// <returns></returns>
        public void ExecuteAbility()
        {
            if(AbilityInstance.CanCastAbility)
            {
                EffectTree.EffectContext ctx = new EffectTree.EffectContext()
                {
                    AttackData = new Utils.AttackContext()
                    {
                        _InitialDirection = transform.forward,
                        _InitialGameObject = gameObject,
                        _InitialPosition = transform.position,

                        _TargetDirection = transform.forward,
                        _TargetGameObject = gameObject,
                        _TargetPosition = transform.position
                    },

                    ContextData = new EffectTree.EffectContext.EffectContextInfo()
                };

                AbilityInstance.ExecuteAbility(ref ctx);
            }
            //return false;
        }

        void OnDestroy()
        {
            if (_AbilityInstance != null) Destroy(_AbilityInstance);
        }
    }
}