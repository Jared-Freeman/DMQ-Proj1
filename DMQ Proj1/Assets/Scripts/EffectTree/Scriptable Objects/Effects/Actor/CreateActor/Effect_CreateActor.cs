using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EffectTree
{
    /// <summary>
    /// Creates Actor(s) at the context location
    /// </summary>
    [CreateAssetMenu(fileName = "CA_", menuName = "Effect Tree/Create Actor", order = 2)]
    public class Effect_CreateActor : Effect_Base
    {
        public GameObject ActorPrefab;
        [Min(0)]
        public int Count;

        /// <summary>
        /// Effect to run on the newly-spawned Actor(s)
        /// </summary>
        public EffectTree.Effect_Base SpawnEffect;

        /// <summary>
        /// random radius the entity spawns in
        /// </summary>
        [Min(0)]        
        public int SpawnRadius; 

        public override bool Invoke(ref EffectContext ctx)
        {
            if (Globals.s_LogEffectTree) Debug.Log(ToString());

            if (base.Invoke(ref ctx))
            {
                for(int i=0; i<Count; i++)
                {
                    Vector3 point;
                    var inst = Instantiate(ActorPrefab);
                    if (Utils.AI.RandomPointInCircle(ctx.AttackData._InitialPosition, 0, SpawnRadius, out point))
                    {
                        inst.transform.position = point;
                    }
                    else inst.transform.position = ctx.AttackData._InitialPosition;
                }
                return true;
            }

            return false;
        }
    }
}