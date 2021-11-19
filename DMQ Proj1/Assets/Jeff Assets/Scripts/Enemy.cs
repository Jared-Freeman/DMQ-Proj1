using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //This class holds stats about the enemies such as health

    public int currentHealth;
    public int maxHealth = 10;
    public Healthbar healthbar;

    void Start()
    {
        currentHealth = maxHealth;
        healthbar.SetMaxHealth(maxHealth);
    }
    // Update is called once per frame
    void Update()
    {
        //For now, press R to do 1 damage to the enemy.
        //This may be later called by an event manager.
        if (Input.GetKeyDown(KeyCode.R))
            TakeDamage(3);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        
        if (currentHealth <= 0)
        {
            //Enemy is dead
            Debug.Log("Enemy is dead");
            gameObject.SetActive(false);
            healthbar.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("Enemy's current health: " + currentHealth);
            healthbar.SetHealth(currentHealth);
        }
    }
}
