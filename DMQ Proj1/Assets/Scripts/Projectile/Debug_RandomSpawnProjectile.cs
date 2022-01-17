using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debug_RandomSpawnProjectile : MonoBehaviour
{
    public bool Spawn = false;

    [Min(.0001f)]
    public float ProjectileSpawnInterval = .01f;

    public GameObject Projectile;

    private void Start()
    {
        if(Projectile == null)
        {
            Debug.LogError(ToString() + ": No Projectile specified. Destroying");
            Destroy(this);
        }
        StartCoroutine(I_Spawn());
    }

    private IEnumerator I_Spawn()
    {
        while(true)
        {
            if(Spawn && Projectile.GetComponent<GenericProjectile>())
            {
                GenericProjectile.SpawnProjectile(Projectile.GetComponent<GenericProjectile>(), transform.position, Random.insideUnitSphere.normalized, Random.insideUnitCircle, null);

                yield return new WaitForSeconds(ProjectileSpawnInterval);
            }
            else yield return null;
        }
    }
}
