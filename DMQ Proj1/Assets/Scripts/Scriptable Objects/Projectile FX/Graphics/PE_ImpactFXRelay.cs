using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PE_", menuName = "ScriptableObjects/ProjectileEffect/Graphics/Play Impact FX", order = 1)]
public class PE_ImpactFXRelay : ProjectileEffect
{

    #region Members

    [System.Serializable]
    public struct EffectOptions
    {
        public bool AdjustFXRotation;

        public List<ImpactFX.ImpactEffect> FX;

    };
    public EffectOptions Options;
    
    #endregion

    public override void PerformPayloadEffect(GenericProjectile Projectile, Collider Col = null)
    {
        base.PerformPayloadEffect(Projectile, Col);

        foreach (var F in Options.FX)
        {
            if (Options.AdjustFXRotation)
            {
                F.SpawnImpactEffect(null, Projectile.gameObject.transform.position, Projectile.gameObject.transform.forward);
            }
            else
            {
                F.SpawnImpactEffect(null, Projectile.gameObject.transform.position);
            }
        }
    }
}
