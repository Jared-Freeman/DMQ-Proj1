using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AbilitySystem
{
    /// <summary>
    /// Attempts to cast the supplied ability when the attached actor is defeated
    /// </summary>
    public class AS_ExecuteAbilityOnActorDeath : MonoBehaviour
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

            actor.OnActorDead_Local += Actor_OnActorDead_Local;
        }

        /// <summary>
        /// Fires only once. Event handler for invoking ability.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Actor_OnActorDead_Local(object sender, CSEventArgs.ActorEventArgs e)
        {
            //
            actor.OnActorDead_Local -= Actor_OnActorDead_Local;

            if (AbilityInstance.CanCastAbility)
            {

                Utils.AttackContext ac = new Utils.AttackContext()
                {
                    _InitialDirection = transform.forward,
                    _InitialPosition = transform.position,
                    _InitialGameObject = gameObject,
                    _TargetDirection = transform.forward,
                    _TargetGameObject = gameObject,
                    _TargetPosition = transform.position,
                    _Team = actor._Team,
                    _Owner = actor
                };

                EffectTree.EffectContext ec = new EffectTree.EffectContext()
                {
                    AttackData = ac,
                    ContextData = new EffectTree.EffectContext.EffectContextInfo()
                };

                AbilityInstance.ExecuteAbility(ref ec);
            }
        }
    }

}