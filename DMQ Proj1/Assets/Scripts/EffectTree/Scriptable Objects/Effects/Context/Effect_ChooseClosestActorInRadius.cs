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


                    newContext.AttackData._TargetGameObject = null;
                    if (actor_cs.Count > 0)
                    {
                        Collider bestCollider = null;
                        if (TargetFilters.TargetIsAllowed(ctx.AttackData._Team, actor_cs[0].gameObject.GetComponent<Actor>()))
                        {
                            bestCollider = actor_cs[0];
                        }


                        Vector3 origin = ctx.AttackData._InitialPosition;
                        foreach (Collider c in actor_cs)
                        {
                            if (
                                (bestCollider.gameObject.transform.position - origin).sqrMagnitude > (c.gameObject.transform.position - origin).sqrMagnitude
                                && TargetFilters.TargetIsAllowed(ctx.AttackData._Team, bestCollider.gameObject.GetComponent<Actor>())
                                )
                            {
                                bestCollider = c;
                            }
                        }

                        if(bestCollider != null) newContext.AttackData._TargetGameObject = bestCollider.gameObject;
                    }

                }

                NextEffect.Invoke(ref newContext);

                return true;
            }
            return false;
        }
    }
}