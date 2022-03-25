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
    }

    private void ActorStats_OnDamageTaken(object sender, ActorSystem.EventArgs.ActorDamageTakenEventArgs e)
    {
        string textMessage = e._DamageMessage._DamageInfo.DamageAmount.ToString();

        var bubble = Text_Bubble.CreateTemporaryTextBubble(textMessage, Preset.TextDurationMax, null, Preset.ColorDamage);

        Vector3 bPos = e._Actor.transform.position;
        bPos.y += 5f;

        bubble.gameObject.transform.position = bPos;
    }
}
