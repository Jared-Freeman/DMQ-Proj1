using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoParticleEmitter : MonoBehaviour
{
    public ParticleSystem particleSyst;

    // Start is called before the first frame update
    void Start()
    {
        if(particleSyst != null)
        {
            var Main = particleSyst.main;
            Main.loop = true;
            particleSyst.Play();
        }
    }

}
