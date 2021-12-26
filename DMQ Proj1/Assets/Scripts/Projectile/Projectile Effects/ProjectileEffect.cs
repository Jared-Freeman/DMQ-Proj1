using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileEffect", menuName = "ScriptableObjects/ProjectileEffect/ProjectileEffectScriptableObject", order = 1)]
public class ProjectileEffect :  ScriptableObject
{
    public float test;

    public virtual void PerformPayloadEffect(GenericProjectile Projectile, Collider Col = null)
    {
        Debug.Log("test: " + test);
    }
}
