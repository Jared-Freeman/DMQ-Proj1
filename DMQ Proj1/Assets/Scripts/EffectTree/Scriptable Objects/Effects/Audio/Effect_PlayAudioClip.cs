using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EffectTree
{
    /// <summary>
    /// Play an audio clip
    /// </summary>
    [CreateAssetMenu(fileName = "AudioClip_", menuName = "Effect Tree/Audio/Play Audio Clip", order = 1)]
    public class Effect_PlayAudioClip : Effect_Base
    {
        public AudioClip clip;
        public enum AudioSourceObjectOptions { Owner, Target}
        /// <summary>
        /// Determines the source of the audio clip 
        /// </summary>
        public AudioSourceObjectOptions options = AudioSourceObjectOptions.Owner;
        public float SourceVolume = 1.0f;
        public float MinDistance = 25.0f;
        public float MaxDistance = 50.0f;
        public override bool Invoke(ref EffectContext ctx)
        {
            if(base.Invoke(ref ctx))
            {
                AudioSource source = new AudioSource();
                switch(options)
                {
                    case AudioSourceObjectOptions.Owner:
                        if(ctx.AttackData._Owner.GetComponent<AudioSource>() == null)
                            source = ctx.AttackData._Owner.gameObject.AddComponent<AudioSource>();
                        else
                            source = ctx.AttackData._Owner.gameObject.GetComponent<AudioSource>();
                        break;
                    case AudioSourceObjectOptions.Target:
                        if (ctx.AttackData._TargetGameObject.GetComponent<AudioSource>() == null)
                            source = ctx.AttackData._TargetGameObject.AddComponent<AudioSource>();
                        else
                            source = ctx.AttackData._TargetGameObject.GetComponent<AudioSource>();
                        break;
                    default:
                        break;
                }
                if (clip)
                {
                    source.clip = clip;
                    source.loop = false;
                    source.volume = SourceVolume;
                    source.Play();
                }
                else
                    Debug.Log("No Audio clip set for " + ctx.AttackData._Owner.name + " ability");

                return true;
            }
            return false;
        }
    }
}

