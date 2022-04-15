using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EffectTree
{
    /// <summary>
    /// Helper component for Effect: <see cref="Effect_PlayAudioClip"/>
    /// </summary>
    /// <remarks>
    /// <para>Should be added to a newly-instantiated, empty gameobject! </para> 
    /// <para>WILL DESTROY THE GAMEOBJECT IT IS ATTACHED TO!</para>
    /// </remarks>
    public class Effect_PlayAudioClip_Helper : MonoBehaviour
    {
        public static float s_RoutineWaitDuration = 2f;
        
        public Effect_PlayAudioClip Preset;
        public EffectContext ctx;

        protected AudioSource AttachedSource;

        void Awake()
        {
            AttachedSource = GetComponent<AudioSource>();
            if (!Utils.Testing.ReferenceIsValid(AttachedSource)) Destroy(this);
        }

        void Start()
        {
            //these are loaded by the Preset SO
            if (!Utils.Testing.ReferenceIsValid(Preset)) Destroy(gameObject);
            if (!Utils.Testing.ReferenceIsValid(ctx)) Destroy(gameObject);

            StartCoroutine(I_WaitToDestroy());
        }

        public IEnumerator I_WaitToDestroy()
        {
            while(AttachedSource.isPlaying)
            {
                yield return new WaitForSeconds(s_RoutineWaitDuration);
            }
            Destroy(gameObject);
        }
    }
}
