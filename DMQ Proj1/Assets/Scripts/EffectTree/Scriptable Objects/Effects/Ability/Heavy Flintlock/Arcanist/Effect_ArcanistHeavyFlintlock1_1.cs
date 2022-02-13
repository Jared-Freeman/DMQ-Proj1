using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EffectTree
{
    [CreateAssetMenu(fileName = "E_", menuName = "Effect Tree/Arcanist/Cursed Bullet Effect", order = 1)]
    public class Effect_ArcanistHeavyFlintlock1_1 : Effect_Base
    {
        public GameObject projectilePrefab;
        public bool UseTeamNoCollideLayer = false;
        private List<GameObject> otherActors;
        public override bool Invoke(ref EffectContext ctx)
        {
            if (base.Invoke(ref ctx))
            {
                var projectile = projectilePrefab.GetComponent<GenericProjectile>();
                if (!projectilePrefab) return false;

                var instance1 = Utils.Projectile.CreateProjectileFromAttackContext(projectile, ctx.AttackData);
                if (!instance1) return false;

                var instance2 = Utils.Projectile.CreateProjectileFromAttackContext(projectile, ctx.AttackData);
                if (!instance2) return false;

                if(otherActors.Count > 0)
                {
                    Team friendlyTeam = ctx.AttackData._Team;
                    int cloestEnemyIndex = 0;
                    float cloestEnemyDistance = 1000000;
                    float distance;
                    Vector3 currentPosition = ctx.AttackData._InitialPosition;
                    //Find the cloest enemy
                    for (int i=0;i<otherActors.Count;i++)
                    {
                        Actor currentActor = otherActors[i].GetComponent<Actor>();
                        if (currentActor)
                        {
                            if(currentActor._Team != friendlyTeam)
                            {
                                
                                Vector3 enemyPosition = otherActors[i].gameObject.transform.position;
                                distance = Vector3.Distance(enemyPosition, currentPosition);
                                if (distance < cloestEnemyDistance)
                                    cloestEnemyIndex = i;
                            }
                        }
                    }
                    if(cloestEnemyIndex > 0)
                    {
                        instance1.Info.Target = otherActors[cloestEnemyIndex];
                        instance2.Info.Target = otherActors[cloestEnemyIndex];
                    }
                }

                if (ctx.AttackData._Team != null)
                {
                    if (UseTeamNoCollideLayer)
                    {
                        if (!Utils.TransformUtils.ChangeLayerOfGameObjectAndChildren(instance1.gameObject, ctx.AttackData._Team.Options.NoCollideLayer))
                        {
                            Debug.LogError("Error converting projectile to team layer! Is layer set within proper bounds?");
                        }
                        if (!Utils.TransformUtils.ChangeLayerOfGameObjectAndChildren(instance2.gameObject, ctx.AttackData._Team.Options.NoCollideLayer))
                        {
                            Debug.LogError("Error converting projectile to team layer! Is layer set within proper bounds?");
                        }
                    }
                    else
                    {
                        if (!Utils.TransformUtils.ChangeLayerOfGameObjectAndChildren(instance1.gameObject, ctx.AttackData._Team.Options.Layer))
                        {
                            Debug.LogError("Error converting projectile to team layer! Is layer set within proper bounds?");
                        }
                        if (!Utils.TransformUtils.ChangeLayerOfGameObjectAndChildren(instance2.gameObject, ctx.AttackData._Team.Options.Layer))
                        {
                            Debug.LogError("Error converting projectile to team layer! Is layer set within proper bounds?");
                        }
                    }
                }
                return true;
            }
            return false;
        }
        private void OnTriggerEnter(Collider col)
        {
            
            if (col.gameObject.GetComponent<Actor>())
            {
                if(!otherActors.Contains(col.gameObject))
                    otherActors.Add(col.gameObject);
            }
        }
    }
}

