using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ProjectileEffect_PhysicsPlayParticleSystemMonoBehaviour : MonoBehaviour
{
    private static float CheckServiceRoutineDuration = 3f;
    private ParticleSystem PS;

    private void Start()
    {
        PS = GetComponent<ParticleSystem>();
        if (PS == null) Debug.LogError("PS Not found!");
    }

    public void PlayParticleSystemOnce(Vector3 Pos)
    {
        transform.position = Pos;

        StartCoroutine(ContinueParticleSystem(Pos));

    }

    public IEnumerator ContinueParticleSystem(Vector3 Pos)
    {
        PS = GetComponent<ParticleSystem>();
        if (PS == null) Debug.LogError("PS Not found!");
        PS.Play();

        while(PS.IsAlive())
            yield return new WaitForSeconds(CheckServiceRoutineDuration);

        Destroy(gameObject);
    }
}

