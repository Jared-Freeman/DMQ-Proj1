using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EffectTree
{
    /// <summary>
    /// Dispatches an Effect to objects in the supplied prefab containing Colliders
    /// </summary>
    [CreateAssetMenu(fileName = "ActorInRadius_", menuName = "Effect Tree/Modify Context/Effect to Collider Area", order = 1)]
    public class Effect_ColliderArea : Effect_Base
    {
        public Effect_Base Effect;
        public Utils.TargetFilterOptions TargetFilters;

        public GameObject ColliderPrefab;

        public EffectContext.PositionOptions PositionOption;

        public override bool Invoke(ref EffectContext ctx)
        {
            if(base.Invoke(ref ctx))
            {
                Vector3 result = new Vector3();

                if (ctx.RetrievePosition(PositionOption, ref result))
                {
                    GameObject go = Instantiate(ColliderPrefab);

                    go.transform.position = result;

                    List<GameObject> List_Overlaps = Utils.Physics.GetOverlappingGameObjects(go);

                    foreach(GameObject g in List_Overlaps)
                    {
                        Actor targetActor = g.GetComponent<Actor>();

                        EffectContext ecInstance = new EffectContext(ctx);
                        ecInstance.AttackData._TargetGameObject = g;
                        ecInstance.AttackData._TargetPosition = g.transform.position;

                        if (targetActor != null)
                        {
                            if(TargetFilters.TargetIsAllowed(ctx.AttackData._Team, targetActor))
                            {
                                Effect.Invoke(ref ecInstance);
                            }    
                        }
                        else
                        {
                            Effect.Invoke(ref ecInstance);
                        }
                    }
                }

                return true;
            }
            return false;
        }
    }
}
