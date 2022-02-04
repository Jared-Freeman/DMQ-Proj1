using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using EffectTree;

namespace CSEventArgs
{
    public class StatusEffect_Actor_EventArgs : System.EventArgs
    {
        public ActorSystem.StatusEffect.SE_StatusEffect_Instance _StatusEffect;
        public Actor _Actor;

        public StatusEffect_Actor_EventArgs(ActorSystem.StatusEffect.SE_StatusEffect_Instance effect, Actor attached_actor)
        {
            _StatusEffect = effect;
            _Actor = attached_actor;
        }
    }
}

namespace ActorSystem.StatusEffect
{
    /// <summary>
    /// An instance of a given status effect. SE_StatusEffect_Base SO's generate these components.
    /// </summary>
    public class SE_StatusEffect_Instance : MonoBehaviour
    {
        #region Members

        public SE_StatusEffect_Base Preset;
        protected SE_Instance_StateInfo Info;
        /// <summary>
        /// Please note this effect context is somewhat limited (targeting is not inferred and may create strange results).
        /// </summary>
        public EffectContext _InstanceContext;

        #region Properties

        public SE_StatusEffect_Base.SE_EffectSettings FX { get { return Preset.Settings.Effects; } }
        public float RemainingDuration { get { return Info.RemainingDuration; } }
        public Actor AttachedActor { get { return Info.AttachedActor; } }

        #endregion

        #region Helpers

        [System.Serializable]
        public struct SE_Instance_StateInfo
        {
            public Actor AttachedActor;

            public float RemainingDuration;
        }

        #endregion

        #endregion

        #region Events

        public static event System.EventHandler<CSEventArgs.StatusEffect_Actor_EventArgs> OnStatusEffectCreate;
        public static event System.EventHandler<CSEventArgs.StatusEffect_Actor_EventArgs> OnStatusEffectDestroy;
        public event System.EventHandler<CSEventArgs.StatusEffect_Actor_EventArgs> OnStatusEffectDestroy_Local;

        #endregion

        #region Initialization

        /// <summary>
        /// Initialization of default values and state info refs.
        /// </summary>
        protected void Awake()
        {
            Info.AttachedActor = gameObject.GetComponent<Actor>();
        }

        protected void Start()
        {
            Info.RemainingDuration = Preset.Settings.Defaults.Duration;

            //guard in case context was generated prior to class Start() invocation. (like if we created this component and assigned some context to it elsewhere)
            if(_InstanceContext == null)
                _InstanceContext = new EffectContext();

            _InstanceContext.AttackData._InitialGameObject = gameObject;
            _InstanceContext.AttackData._Owner = Info.AttachedActor;
            if(Info.AttachedActor != null) _InstanceContext.AttackData._Team = Info.AttachedActor._Team;

            UpdateEffectContext();

            FX.Effect_Begin?.Invoke(ref _InstanceContext);

            //Avoid periodic coroutine invocation if no periodic effect exists!
            if(FX.Effect_Periodic != null) StartCoroutine(I_PeriodicUpdate());

            OnStatusEffectCreate?.Invoke(this, new CSEventArgs.StatusEffect_Actor_EventArgs(this, AttachedActor));
        }

        #endregion

        void FixedUpdate()
        {
            Info.RemainingDuration -= Time.fixedDeltaTime;
            if(Info.RemainingDuration <= 0)
            {
                Destroy(this);
            }
        }

        protected void UpdateEffectContext()
        {
            _InstanceContext.AttackData._InitialDirection = gameObject.transform.forward;
            _InstanceContext.AttackData._InitialPosition = gameObject.transform.position;
        }

        protected IEnumerator I_PeriodicUpdate()
        {
            yield return new WaitForSeconds(Preset.Settings.Effects.PeriodicEffectInterval);

            while(Preset.Settings.Flags.InfiniteDuration || RemainingDuration > 0)
            {
                UpdateEffectContext();
                FX.Effect_Periodic?.Invoke(ref _InstanceContext);
                yield return new WaitForSeconds(Preset.Settings.Effects.PeriodicEffectInterval);
            }

            UpdateEffectContext();
            FX.Effect_End?.Invoke(ref _InstanceContext);
        }

        protected void OnDestroy()
        {
            FX.Effect_Destroyed?.Invoke(ref _InstanceContext);

            OnStatusEffectDestroy?.Invoke(this, new CSEventArgs.StatusEffect_Actor_EventArgs(this, AttachedActor));
            OnStatusEffectDestroy_Local?.Invoke(this, new CSEventArgs.StatusEffect_Actor_EventArgs(this, AttachedActor));
        }
    }
}