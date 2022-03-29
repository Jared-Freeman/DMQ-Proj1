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

        public AS_ExecuteAbilityOnPhysicCollisionPreset Preset;

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

        void Awake()
        {
            //Validate refs we need
            if (!Utils.Testing.ReferenceIsValid(Preset)) Destroy(this);
        }

        void Start()
        {
            //these can be null (aka no ability exists in preset).
            if (Preset.Ability_CollisionEnter) _AbilityInstanceEnter = Preset.Ability_CollisionEnter?.GetInstance(gameObject);
            if (Preset.Ability_CollisionStay) _AbilityInstanceStay = Preset.Ability_CollisionStay?.GetInstance(gameObject);
            if (Preset.Ability_CollisionExit) _AbilityInstanceExit = Preset.Ability_CollisionExit?.GetInstance(gameObject);
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
