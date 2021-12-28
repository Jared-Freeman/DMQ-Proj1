using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileEffect_PhysicsExplosiveImpulse", menuName = "ScriptableObjects/ProjectileEffect/Graphics/Play Particle System", order = 1)]
public class ProjectileEffect_PhysicsPlayParticleSystem : ProjectileEffect
{
    #region Members

    [System.Serializable]
    public struct EffectOptions
    {
        public ParticleSystem ParticleSystemToPlay;

    };
    public EffectOptions Options;

    #endregion


    public override void PerformPayloadEffect(GenericProjectile Projectile, Collider Col = null)
    {
        base.PerformPayloadEffect(Projectile, Col);

        if(Options.ParticleSystemToPlay != null)
        {
            Vector3 EffectPosition = Projectile.transform.position;

        }
        else
        {
            Debug.LogError("ProjectileEffect_PhysicsPlayParticleSystem Asset must have a ParticleSystem specified!");
        }
    }
}
