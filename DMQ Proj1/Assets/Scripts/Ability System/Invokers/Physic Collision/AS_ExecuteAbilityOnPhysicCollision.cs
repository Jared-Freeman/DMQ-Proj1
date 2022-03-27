using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem
{
    /// <summary>
    /// Attempts to cast the supplied ability when the attached actor Stats takes damage.
    /// </summary>
    public class AS_ExecuteAbilityOnPhysicCollision : MonoBehaviour
    {
        #region Members

        //presets
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

        //instances
        protected AbilitySystem.AS_Ability_Instance_Base _AbilityInstanceEnter;
        protected AbilitySystem.AS_Ability_Instance_Base _AbilityInstanceStay;
        protected AbilitySystem.AS_Ability_Instance_Base _AbilityInstanceExit;

        #region Properties

        //ability instance property public-accessors
        public AbilitySystem.AS_Ability_Instance_Base AbilityInstanceEnter { get => _AbilityInstanceEnter; }
        public AbilitySystem.AS_Ability_Instance_Base AbilityInstanceStay { get => _AbilityInstanceStay; }
        public AbilitySystem.AS_Ability_Instance_Base AbilityInstanceExit { get => _AbilityInstanceExit; }

        #endregion

        #endregion

        void Start()
        {
            _AbilityInstanceEnter = Ability_CollisionEnter.GetInstance(gameObject);
            _AbilityInstanceStay = Ability_CollisionStay.GetInstance(gameObject);
            _AbilityInstanceExit = Ability_CollisionExit.GetInstance(gameObject);
        }

        void OnCollisionEnter(Collision c)
        {
            EffectTree.EffectContext ctx = new EffectTree.EffectContext();
            ctx.ContextData = EffectTree.EffectContext.CreateContextDataFromCollision(c);
            ctx.AttackData = EffectTree.EffectContext.CreateAttackContextDataFromCollision(c);

            _AbilityInstanceEnter?.ExecuteAbility(ref ctx);
        }
        void OnCollisionStay(Collision c)
        {
            EffectTree.EffectContext ctx = new EffectTree.EffectContext();
            ctx.ContextData = EffectTree.EffectContext.CreateContextDataFromCollision(c);
            ctx.AttackData = EffectTree.EffectContext.CreateAttackContextDataFromCollision(c);

            AbilityInstanceStay?.ExecuteAbility(ref ctx);
        }
        void OnCollisionExit(Collision c)
        {
            EffectTree.EffectContext ctx = new EffectTree.EffectContext();
            ctx.ContextData = EffectTree.EffectContext.CreateContextDataFromCollision(c);
            ctx.AttackData = EffectTree.EffectContext.CreateAttackContextDataFromCollision(c);

            AbilityInstanceExit?.ExecuteAbility(ref ctx);
        }
    }
}
