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
        public enum AudioSourceObjectOptions { Owner, Target, None }
        /// <summary>
        /// Determines the source of the audio clip 
        /// </summary>
        public AudioSourceObjectOptions options = AudioSourceObjectOptions.Owner;
        public float SourceVolume = 1.0f;
        public float MinDistance = 25.0f;
        public float MaxDistance = 50.0f;
        public float clipStartTime = 0.0f;
        public override bool Invoke(ref EffectContext ctx)
        {
            if(base.Invoke(ref ctx))
            {
                if (clip == null)
                {
                    Debug.Log("No Audio clip set for " + ctx.AttackData._Owner.name + " ability");
                    return false;
                }

                //TODO: Make a cookie monobehavior for this instead of replacing previous sounds
                AudioSource source;/* = new AudioSource();*/
                GameObject hostGO = new GameObject("Audio Clip Host");
                source = hostGO.AddComponent<AudioSource>();

                Effect_PlayAudioClip_Helper helper = hostGO.AddComponent<Effect_PlayAudioClip_Helper>();
                helper.ctx = ctx;
                helper.Preset = this;

                switch (options)
                {
                    case AudioSourceObjectOptions.Owner:
                        if (ctx.AttackData._Owner != null)
                            hostGO.transform.parent = ctx.AttackData._Owner.gameObject.transform;
                        break;
                    case AudioSourceObjectOptions.Target:
                        if (ctx.AttackData._TargetGameObject != null)
                            hostGO.transform.parent = ctx.AttackData._TargetGameObject.transform;
                        break;
                    case AudioSourceObjectOptions.None:
                        break;
                    default:
                        Debug.Log("No AudioSourceObjectOptions impl found! Does an impl exist?");
                        return false;
                }

                source.clip = clip;
                source.loop = false;
                source.volume = SourceVolume;
                source.time = clipStartTime;
                source.Play();

                return true;
            }
            return false;
        }
    }
}

