using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    //public Material healthBar;

    public MaterialPropertyBlock matProperties;
    protected SpriteRenderer _Renderer;

    [Range(0, 1)]
    public float _Alpha = 1.0f;

    public float maxHealth { get; set; }
    public float curHealth { get; set; }
    public float maxHealthTotal { get; set; }
    private float healthPercentage;

    private float red = 0.0f;
    private float green = 1.0f;
    private float blue = 0.0f;
    private Color color = new Color(0.0f, 1.0f, 0.0f, 1.0f);

    void Awake()
    {
        _Renderer = GetComponent<SpriteRenderer>();
        if (_Renderer == null) Destroy(this);

        matProperties = new MaterialPropertyBlock();
    }

    void Start()
    {
        matProperties.SetFloat("_Health", curHealth);
        matProperties.SetFloat("_CurrentMaxHealth", maxHealth);
        matProperties.SetFloat("_MaximumHealth", maxHealthTotal);
        matProperties.SetColor("_Color", color);
    }

    void Update()
    {
        updateHealthColor();

        _Renderer.SetPropertyBlock(matProperties);
    }

    void takeDamage()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            curHealth -= 3;
            matProperties.SetFloat("_Health", curHealth);
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
        color = new Color(red, green, blue, _Alpha);

        matProperties.SetColor("_Color", color);

        matProperties.SetFloat("_Health", curHealth);
        matProperties.SetFloat("_CurrentMaxHealth", maxHealth);
        matProperties.SetFloat("_MaximumHealth", maxHealthTotal);
        matProperties.SetColor("_Color", color);
    }
}
