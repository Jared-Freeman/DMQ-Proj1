using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace EffectTree
{
    /// <summary>
    /// Play a Particle System 
    /// </summary>
    [CreateAssetMenu(fileName = "PartSyst_", menuName = "Effect Tree/Graphics/Play Particle System", order = 2)]
    public class Effect_PlayParticleSystem : Effect_Base
    {

        /// <summary>
        /// The Particle System's local forward transform (blue arrow) will be rotated to face this forward vector
        /// </summary>
        [Tooltip("Local blue arrow of Particle System GameObject rotates to face this vector.")]
        public EffectContext.FacingOptions ForwardVectorSelection;

        /// <summary>
        /// Just in case the direction is backwards.
        /// </summary>
        public bool FlipDirection = false;

        /// <summary>
        /// Where the ParticleSystem will spawn. 
        /// </summary>
        /// <remarks>
        /// We use a bit of inference to find a suitable alternative, should our desired position not exist in the ctx.
        /// </remarks>
        public EffectContext.PositionOptions PlayPosition = EffectContext.PositionOptions._InitialPosition;

        /// <summary>
        /// A GameObject with a Particle System on it. Doesnt need any Helper component.
        /// </summary>
        public GameObject ParticleSystemPrefab;

        public override bool Invoke(ref EffectContext ctx)
        {
            if( base.Invoke(ref ctx))
            {
                GameObject g = Instantiate(ParticleSystemPrefab);

                g.name = "[EffectTree] Particle System Instance";

                //rotate gameobject
                Vector3 fwd = ctx.RetrieveDirectionVector(ForwardVectorSelection);
                if (fwd.sqrMagnitude <= 0) fwd = ctx.GetAnyDirectionVector();
                if (FlipDirection) fwd *= -1f;
                g.transform.forward = fwd.normalized;

                //position gameobject
                Vector3 startPos = Vector3.zero;

                //position retrieval from ctx
                bool successful = false;
                if (!ctx.RetrievePosition(PlayPosition, ref startPos))
                {
                    //contextual inference. Here I "guess" that the next best option is to use the target position.
                    if(PlayPosition == EffectContext.PositionOptions.CollisionImpactPoint)
                    {
                        successful = ctx.RetrievePosition(EffectContext.PositionOptions._TargetPosition, ref startPos);
                    }          
                    
                    //one last hail mary lol
                    if(!successful)
                    {
                        ctx.RetrievePosition(EffectContext.PositionOptions._InitialPosition, ref startPos);
                    }
                }
                //assign pos
                g.transform.position = startPos;

                //attach a helper, or retrieve it if it already exists.
                Effect_PlayParticleSystem_Helper h = g.GetComponent<Effect_PlayParticleSystem_Helper>();
                if (h == null) h = g.AddComponent<Effect_PlayParticleSystem_Helper>();
                h.Preset = this;

                return true;
            }
            return false;
        }
    }

}
