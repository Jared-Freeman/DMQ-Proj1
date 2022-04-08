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

    protected void GenerateDamageProjectile(string message, Vector3 position, Color color)
    {

        var instProjectile = CreateProjectileInstanceFromPrefab(position);

        if (instProjectile != null)
        {
            var bubble = Text_Bubble.CreateTemporaryTextBubble(message, Preset.TextDurationMax, instProjectile.gameObject, color);

            //bubble.gameObject.transform.position = new Vector3(0, 0, 0);

            bubble.text_mesh_pro.fontSize = Preset.FontSizeMax;

            var rectTransform = bubble.transform as RectTransform;
            if (rectTransform != null)
            {
                bubble.transform.SetParent(instProjectile.gameObject.transform, false);
                //rectTransform.anchoredPosition3D = new Vector3(0, 0, 0);
            }
            else
            {
                bubble.transform.parent = instProjectile.gameObject.transform;
            }
            bubble.transform.localPosition = new Vector3(0, 0, 0);
        }
    }

    private void ActorStats_OnHealingReceived(object sender, ActorSystem.EventArgs.ActorHealingReceivedEventArgs e)
    {
        string textMessage = e._Modifiers.HP.Modifier.Add.ToString();

        Vector3 bPos = e._Actor.transform.position;
        bPos.y += 5f;

        GenerateDamageProjectile(textMessage, bPos, Preset.ColorHealing);
    }


    private void ActorStats_OnDamageTaken(object sender, ActorSystem.EventArgs.ActorDamageTakenEventArgs e)
    {
        string textMessage = e._DamageMessage._DamageInfo.DamageAmount.ToString();

        Vector3 bPos = e._Actor.transform.position;
        bPos.y += 5f;

        GenerateDamageProjectile(textMessage, bPos, Preset.ColorDamage);
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
