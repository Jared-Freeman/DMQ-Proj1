using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public Material healthBar;
    public float maxHealth;
    public float curHealth;
    public float maxHealthTotal;
    public float healthPercentage;

    private float red = 0.0f;
    private float green = 1.0f;
    private float blue = 0.0f;
    private float albedo = 1.0f;
    private Color color = new Color(0.0f, 1.0f, 0.0f, 1.0f);

    void Start()
    {
        healthBar.SetFloat("_Health", curHealth);
        healthBar.SetFloat("_CurrentMaxHealth", maxHealth);
        healthBar.SetFloat("_MaximumHealth", maxHealthTotal);
        healthBar.SetColor("_Color", color);
    }

    void Update()
    {
        takeDamage();
    }

    void takeDamage()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            curHealth -= 3;
            healthBar.SetFloat("_Health", curHealth);
        }
        updateHealthColor();
    }

    void updateHealthColor()
    {
        healthPercentage = curHealth / maxHealth;
        if (healthPercentage >= 0.5f)
        {
            red = (1 - healthPercentage) * 2; 
            green = 1.0f;
        }
        else
        {
            red = 1.0f;
            green = 1.0f - ((1.0f - healthPercentage));
        }
        color = new Color(red, green, blue, albedo);
        healthBar.SetColor("_Color", color);
    }
}
