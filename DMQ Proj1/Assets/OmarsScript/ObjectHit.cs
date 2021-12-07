using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHit : MonoBehaviour
{
    [SerializeField] ParticleSystem bumpedWall;
    //Need to figure out how to add sound and trigger only when collision is detected
    void OnCollisionEnter(Collision other){
        Debug.Log("Bumped Into Wall");
        bumpedWall.Play();
        Destroy(gameObject,0.2f);
    }
}