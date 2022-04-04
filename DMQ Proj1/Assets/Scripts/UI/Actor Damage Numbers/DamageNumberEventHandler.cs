using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Listens to actor take damage events and creates text bubbles to display the damage amount
/// </summary>
public class DamageNumberEventHandler : MonoBehaviour
{
    public DamageNumberEventHandlerPreset Preset;
    
    void Awake()
    {
        if(Preset == null)
        {
            Debug.LogError("No Preset found.");
            Destroy(this);
        }
    }

    void Start()
    {
        ActorStats.OnDamageTaken += ActorStats_OnDamageTaken;
        ActorStats.OnHealingReceived += ActorStats_OnHealingReceived;
    }

    private void ActorStats_OnHealingReceived(object sender, ActorSystem.EventArgs.ActorHealingReceivedEventArgs e)
    {
        string textMessage = e._Modifiers.HP.Modifier.Add.ToString();

        var bubble = Text_Bubble.CreateTemporaryTextBubble(textMessage, Preset.TextDurationMax, null, Preset.ColorHealing);

        Vector3 bPos = e._Actor.transform.position;
        bPos.y += 5f;

        bubble.gameObject.transform.position = bPos;

        bubble.text_mesh_pro.fontSize = Preset.FontSizeMax;

        var instProjectile = CreateProjectileInstanceFromPrefab(bPos);
        if (instProjectile != null) bubble.transform.parent = instProjectile.gameObject.transform;
    }


    private void ActorStats_OnDamageTaken(object sender, ActorSystem.EventArgs.ActorDamageTakenEventArgs e)
    {
        string textMessage = e._DamageMessage._DamageInfo.DamageAmount.ToString();

        var bubble = Text_Bubble.CreateTemporaryTextBubble(textMessage, Preset.TextDurationMax, null, Preset.ColorDamage);

        Vector3 bPos = e._Actor.transform.position;
        bPos.y += 5f;

        bubble.gameObject.transform.position = bPos;

        bubble.text_mesh_pro.fontSize = Preset.FontSizeMax;

        var instProjectile = CreateProjectileInstanceFromPrefab(bPos);
        if (instProjectile != null) bubble.transform.parent = instProjectile.gameObject.transform;
    }
    protected GenericProjectile CreateProjectileInstanceFromPrefab(Vector3 initialPosition)
    {
        if (Preset.BubbleParentPrefab != null)
        {
            GenericProjectile gp = Preset.BubbleParentPrefab.GetComponent<GenericProjectile>();
            if (gp != null)
            {
                var ctx = new EffectTree.EffectContext()
                {
                    AttackData = new Utils.AttackContext()
                    {
                        _InitialDirection = Vector3.up,
                        _InitialPosition = initialPosition,
                        _InitialGameObject = null
                    }
                };

                var instProjectile = Utils.Projectile.CreateProjectileFromEffectContext(gp, ctx, EffectTree.Effect_LaunchProjectile.SpawnContextOptions.InitialDirection);

                var rb = instProjectile.GetComponent<Rigidbody>();
                if(rb != null && Preset.RandomXZForceMax > 0)
                {
                    Vector2 xzRand = Random.insideUnitCircle;
                    rb.AddForce(xzRand * Preset.RandomXZForceMax, ForceMode.Impulse);
                }

                // change layer to nocollide?
                //Utils.TransformUtils.ChangeLayerOfGameObjectAndChildren(instProjectile.gameObject, 30);

                return instProjectile;
            }
        }
        return null;

    }
}
