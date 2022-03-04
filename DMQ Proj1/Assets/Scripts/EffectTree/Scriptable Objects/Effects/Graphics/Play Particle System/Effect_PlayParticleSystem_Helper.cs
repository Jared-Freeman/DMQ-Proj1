using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EffectTree
{
    /// <summary>
    /// Helper for <see cref="Effect_PlayParticleSystem"/>
    /// </summary>
    public class Effect_PlayParticleSystem_Helper : MonoBehaviour
    {
        private static float _RoutineWaitForDestroySeconds = 2f;

        public Effect_PlayParticleSystem Preset;
        private ParticleSystem _P;

        private void Awake()
        {
            _P = GetComponent<ParticleSystem>();
            if (_P == null) Destroy(gameObject);

            //var temp = _P.main;

            //temp.duration = IFX.Options.Duration;
            //temp.loop = false;
        }

        private void Start()
        {
            if (Preset == null) Destroy(gameObject);

            StartCoroutine(Action());
        }

        private IEnumerator Action()
        {
            _P.Play();

            yield return new WaitForSeconds(_P.main.duration);

            _P.Stop();

            while (_P.particleCount > 0)
            {
                yield return new WaitForSeconds(_RoutineWaitForDestroySeconds);
            }

            Destroy(gameObject);
        }
    }
}