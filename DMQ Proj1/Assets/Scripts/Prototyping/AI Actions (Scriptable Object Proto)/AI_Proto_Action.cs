using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Actions operate on their AttachedLogic, and may contain additional state variables, etc when needed
public class AI_Proto_Action : ScriptableObject
{
    protected AI_Proto_Logic AttachedLogic;

    public virtual void Invoke(AI_Proto_Logic Logic)
    {
        AttachedLogic = Logic;
    }
}