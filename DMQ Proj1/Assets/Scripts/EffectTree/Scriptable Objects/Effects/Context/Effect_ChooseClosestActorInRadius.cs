using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EffectTree
{
    /// <summary>
    /// Finds the nearest Actor in the specified radius (from InitialPosition)
    /// </summary>
    [CreateAssetMenu(fileName = "ActorInRadius_", menuName = "Effect Tree/Modify Context/Choose Closest Actor in Radius", order = 1)]
    public class Effect_ChooseClosestActorInRadius : Effect_Base
    {
        public float Radius;
        public Effect_Base NextEffect;
        public Utils.TargetFilterOptions TargetFilters;

        public override bool Invoke(ref EffectContext ctx)
        {
            if( base.Invoke(ref ctx) && NextEffect != null)
            {
                Collider[] cs = Physics.OverlapSphere(ctx.AttackData._InitialPosition, Radius);

                EffectContext newContext = ctx;

                if (cs.Length > 0)
                {
                    List<Collider> actor_cs = new List<Collider>();
                    foreach (Collider c in cs)
                    {
                        if (c.gameObject.GetComponent<Actor>() != null) actor_cs.Add(c);
                    }


                    if(actor_cs.Count > 0)
                    {
                        Collider bestCollider = actor_cs[0];
                        Vector3 origin = ctx.AttackData._InitialPosition;
                        foreach (Collider c in actor_cs)
                        {
                            if ((bestCollider.gameObject.transform.position - origin).sqrMagnitude > (c.gameObject.transform.position - origin).sqrMagnitude)
                            {
                                bestCollider = c;
                            }
                        }

                        newContext.AttackData._TargetGameObject = bestCollider.gameObject;
                    }
                    else
                    {

                        newContext.AttackData._TargetGameObject = null;
                    }


                }

                NextEffect.Invoke(ref newContext);

                return true;
            }
            return false;
        }
    }
}