using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class holds the reigns of any AI. Its goal is to take the ActorAI data structure and drive it during game simulation.
[RequireComponent(typeof(ActorAI))]
public class ActorAI_Logic : MonoBehaviour
{
    #region members

    ActorAI AttachedActor;

    #endregion

    private void Start()
    {
        AttachedActor = GetComponent<ActorAI>();
    }

    public void UpdateLogic()
    {

    }
}
