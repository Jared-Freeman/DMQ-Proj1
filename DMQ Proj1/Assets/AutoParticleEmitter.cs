using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoParticleEmitter : MonoBehaviour
{
    public ParticleSystem particleSyst;

    // Start is called before the first frame update
    void Start()
    {
        particleSyst.loop = true;
        particleSyst.Play();
    }

}
