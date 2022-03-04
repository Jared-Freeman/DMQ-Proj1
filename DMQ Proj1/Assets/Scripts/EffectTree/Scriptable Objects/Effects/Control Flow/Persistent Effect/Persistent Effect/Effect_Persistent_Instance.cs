using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EffectTree
{    
    public class Effect_Persistent_Instance : MonoBehaviour
    {
        /// <summary>
        /// Creates a GameObject with an Effect_Persistent_Instance constructed from the supplied args
        /// </summary>
        /// <param name="preset"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public static GameObject CreateInstance(Effect_Persistent preset, EffectContext ctx, GameObject parent = null)
        {
            GameObject g = new GameObject("Effect Persistent Instance");

            var effectInstance = g.AddComponent<Effect_Persistent_Instance>();

            effectInstance.Preset = preset;
            effectInstance.Context = new EffectContext(ctx.AttackData);

            if (parent != null)
            {
                g.transform.parent = parent.transform;
            }

            return g;
        }

        public Effect_Persistent Preset;
        public EffectContext Context;

        /// <summary>
        /// Updates Initial values in context to reflect the Initial Gameobject's current transform
        /// </summary>
        /// <returns></returns>
        EffectContext CreateContext()
        {
            EffectContext ctx = new EffectContext(Context.AttackData);

            if (ctx.AttackData._InitialGameObject != null)
            {
                ctx.AttackData._InitialDirection = ctx.AttackData._InitialGameObject.transform.forward;
                ctx.AttackData._InitialPosition = ctx.AttackData._InitialGameObject.transform.position;
            }

            return ctx;
        }

        void Start()
        {
            foreach (var e in Preset.List_StartEffects)
            {
                var c = CreateContext();
                e.Invoke(ref c);
            }

            if (Preset.Duration > 0) StartCoroutine(DestroyAfterSeconds(Preset.Duration));
            if (Preset.Period > 0) StartCoroutine(DoPeriodicEffect(Preset.Period));
        }

        protected IEnumerator DestroyAfterSeconds(float duration)
        {
            yield return new WaitForSeconds(duration);

            foreach (var e in Preset.List_DestroyEffects)
            {
                var c = CreateContext();
                e.Invoke(ref c);
            }

            Destroy(gameObject);
        }
        protected IEnumerator DoPeriodicEffect(float duration)
        {
            while (true)
            {
                yield return new WaitForSeconds(duration);

                foreach (var e in Preset.List_PeriodicEffects)
                {
                    var c = CreateContext();
                    e.Invoke(ref c);
                }
            }
        }
    }

}