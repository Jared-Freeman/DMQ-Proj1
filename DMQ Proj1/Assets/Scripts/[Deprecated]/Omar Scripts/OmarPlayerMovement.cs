using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OmarPlayerMovement_2 : MonoBehaviour
{
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] GameObject[] fires;
    AudioSource audioSource;
    void Start(){
         
     //FireShot();
    }

    void Update(){
        
       PlayerControls();
       ProcessFiring();
       //ActivateFire();
       
    }
    void ProcessFiring(){
        
        if(Input.GetButton("Fire1")){
            Debug.Log("Firing");
            ActivateFire();
        }
        else{
            DeactivateFire();
            Debug.Log("Not Firing");
        }
    } 
    void ActivateFire(){
        foreach (GameObject fire in fires){
           var emissionModule = fire.GetComponent<ParticleSystem>().emission;
            emissionModule.enabled = true;
        }
    }
    void DeactivateFire(){
        foreach (GameObject fire in fires){
            var emissionModule = fire.GetComponent<ParticleSystem>().emission;
            emissionModule.enabled = false;
        }
    }
    void PlayerControls(){
        
        float xValue = Input.GetAxis("Horizontal")*Time.deltaTime*moveSpeed;
        float zValue = Input.GetAxis("Vertical")*Time.deltaTime*moveSpeed;

        /*if(Input.GetKey(KeyCode.A)){
            audioSource.Play();
        }*/
        transform.Translate(xValue,0,zValue);
    }
}
/*void FireShot(){
    audioSource = GetComponent<AudioSource>();
}*/