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
        public GameObject PrefabWithParticleSystem;

    };
    public EffectOptions Options;

    private GameObject ParticleSystemHost;
    private ParticleSystem PS_Instance;
    private ProjectileEffect_PhysicsPlayParticleSystemMonoBehaviour HelperMonobehaviour;

    #endregion


    public override void PerformPayloadEffect(GenericProjectile Projectile, Collider Col = null)
    {
        base.PerformPayloadEffect(Projectile, Col);

        if(Options.PrefabWithParticleSystem != null)
        {
            Vector3 EffectPosition = Projectile.transform.position;
            
            ParticleSystemHost = GameObject.Instantiate(Options.PrefabWithParticleSystem);
            ParticleSystemHost.name = "ParticleSystemHost";
            PS_Instance = ParticleSystemHost.GetComponent<ParticleSystem>();

            if (ParticleSystemHost.GetComponent<ParticleSystem>() == null) Debug.LogError("NULL");

            HelperMonobehaviour = ParticleSystemHost.AddComponent<ProjectileEffect_PhysicsPlayParticleSystemMonoBehaviour>();

            HelperMonobehaviour.PlayParticleSystemOnce(EffectPosition);
        }
        else
        {
            Debug.LogError("ProjectileEffect_PhysicsPlayParticleSystem Asset must have a ParticleSystem specified!");
        }
    }

}
