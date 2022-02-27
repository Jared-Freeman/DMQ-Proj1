using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetHit : MonoBehaviour{

    [SerializeField] ParticleSystem hit;
    void OnParticleCollision(GameObject other){
        Debug.Log($" I'm hit by! {other.gameObject.name}");
        hit.Play();
        Destroy(gameObject,2f);
    }
}