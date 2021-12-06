using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor_Player : Actor
{
    public PlayerControler controller;
    public ActorAction_Attack attack;
    // Start is called before the first frame update
    new void Start()
    {
        controller = GetComponent<PlayerControler>(); 
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
        //Replace with event interrupt system later
        if (controller.anim.GetBool("AttackTrigger")){
            attack.BeginAttack(true);
        }
    }
}
