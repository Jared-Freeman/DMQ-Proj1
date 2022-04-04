using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class ActorAI : Actor
{
    #region members

    protected ActorAI_Logic Logic { get; private set; }

    /// <summary>
    /// Reference to the ragdoll this actor will use. Can be left null for no ragdoll
    /// </summary>
    public GameObject Ragdoll;

    #endregion

    protected override void Awake()
    {
        base.Awake();
        Logic = GetComponent<ActorAI_Logic>();
        if (!Utils.Testing.ReferenceIsValid(Logic)) Destroy(gameObject);
    }

    public override void ActorDead()
    {

        //spawn ragdoll if one exists
        if(Ragdoll != null)
        {
            GameObject go = Instantiate(Ragdoll);

            go.transform.position = transform.position;
            go.transform.rotation = transform.rotation;

            List<Rigidbody> list_RBs = new List<Rigidbody>();

            ContinueTraverseRagdollTransformCreation(transform, go.transform, ref list_RBs);

            Debug.Log("RBS: " + list_RBs.Count.ToString());

            foreach(var rb in list_RBs)
            {
                rb.AddExplosionForce(8f, transform.position + new Vector3(0,2,0) + rb.velocity.normalized * -2f, 100f, 0, ForceMode.VelocityChange);
            }

            //Transform actorChild, ragdollChild;
            //for(int i=0; i < transform.childCount; i++)
            //{
            //    actorChild = transform.GetChild(i);
            //    ragdollChild = go.transform.GetChild(i);
            //    if (ragdollChild != null)
            //    {
            //        Debug.Log("this is working!!!!!!");
            //        ragdollChild.localPosition = actorChild.localPosition;
            //        ragdollChild.localRotation = actorChild.localRotation;
            //        ragdollChild.localScale = actorChild.localScale;
            //    }
            //}
        }

        base.ActorDead();
    }

    protected void ContinueTraverseRagdollTransformCreation(Transform actorT, Transform ragdollT, ref List<Rigidbody> list_RBs)
    {
        if (actorT == null || ragdollT == null) return;

        Transform actorChild, ragdollChild;
        for (int i = 0; i < actorT.childCount; i++)
        {
            actorChild = actorT.GetChild(i);
            if(ragdollT.childCount > i)
            {
                ragdollChild = ragdollT.GetChild(i);
                ragdollChild.localPosition = actorChild.localPosition;
                ragdollChild.localRotation = actorChild.localRotation;
                ragdollChild.localScale = actorChild.localScale;

                Rigidbody rRB = ragdollChild.GetComponent<Rigidbody>();
                if(rRB != null)
                {
                    list_RBs.Add(rRB);
                }

                //Debug.Log(ragdollChild.name + ", " + actorChild.name);

                ContinueTraverseRagdollTransformCreation(actorChild, ragdollChild, ref list_RBs);
            }
        }
    }
}
