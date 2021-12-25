using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericProjectile : MonoBehaviour
{
    public GenericProjectileMover Mover;

    void Start()
    {
        Mover = GetComponent<GenericProjectileMover>();
        if (Mover == null) Mover = new GenericProjectileMover();
    }

    void Update()
    {

    }
}
