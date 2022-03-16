using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActorSystem.StatusEffect
{
    /// <summary>
    /// Attempts to cast the supplied ability when the attached actor Stats takes damage.
    /// </summary>
    public class AS_ExecuteAbilityOnTakeDamage : MonoBehaviour
    {
        public AbilitySystem.AS_Ability_Base Ability;
        public AbilitySystem.AS_Ability_Instance_Base AbilityInstance { get => _AbilityInstance; }

        //protected members
        protected AbilitySystem.AS_Ability_Instance_Base _AbilityInstance;
        protected Actor actor;
        protected ActorStats actorStats;

        void Awake()
        {
            if (Ability == null) Destroy(this);

            actor = GetComponent<Actor>();
            if (actor == null) Destroy(this);
            actorStats = GetComponent<ActorStats>();
            if (actorStats == null) Destroy(this);
        }

        // Start is called before the first frame update
        void Start()
        {
            _AbilityInstance = Ability.GetInstance(gameObject);


            ActorStats.OnDamageTaken += ActorStats_OnDamageTaken;
        }

        private void ActorStats_OnDamageTaken(object sender, EventArgs.ActorDamageTakenEventArgs e)
        {
            if(e._Actor == actor && _AbilityInstance.CanCastAbility)
            {
                Utils.AttackContext ac = new Utils.AttackContext()
                {
                    _InitialDirection = transform.forward,
                    _InitialPosition = actor.transform.position,
                    _InitialGameObject = gameObject,
                    _TargetDirection = (actor.transform.position - e._DamageMessage._DamageSource.transform.position).normalized,
                    _TargetGameObject = e._DamageMessage._DamageSource,
                    _TargetPosition = e._DamageMessage._DamageSource.transform.position,
                    _Team = actor._Team,
                    _Owner = actor
                };

                EffectTree.EffectContext ec = new EffectTree.EffectContext()
                {
                    AttackData = ac,
                    ContextData = new EffectTree.EffectContext.EffectContextInfo()
                };

                _AbilityInstance.ExecuteAbility(ref ec);
            }
        }

        void OnDestroy()
        {
            if (_AbilityInstance != null) Destroy(_AbilityInstance);
        }
    }
}
