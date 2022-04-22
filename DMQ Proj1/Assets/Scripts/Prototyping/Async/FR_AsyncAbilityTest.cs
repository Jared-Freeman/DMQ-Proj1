using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Init an ability then kill it early to see if syst is working
/// </summary>
public class FR_AsyncAbilityTest : MonoBehaviour
{
    AbilitySystem.AS_Ability_Instance_Base inst;

    void Start()
    {
        StartCoroutine(I_LateGetInst());
    }
    void OnDestroy()
    {
        inst.Cooldown.OnCooldownUsed -= Cooldown_OnCooldownUsed;
    }

    //cast on first frame we can
    void Update()
    {
        if(!inst) { return; }


        if (inst.CanCastAbility)
        {
            print("inst ability cast");
            var c = new EffectTree.EffectContext()
            {
                AttackData = new Utils.AttackContext(),
                ContextData = new EffectTree.EffectContext.EffectContextInfo()
            };
            inst.ExecuteAbility(ref c);
        }
    }

    private void Cooldown_OnCooldownUsed(object sender, Utils.CooldownTracker.CooldownTrackerEventArgs e)
    {
        print("starting early destroy");

        StartCoroutine(I_EarlyKill());
    }

    private IEnumerator I_EarlyKill()
    {
        yield return null;
        yield return null;
        yield return null;

        print("early destroy");
        Destroy(inst);
    }
    private IEnumerator I_LateGetInst()
    {
        yield return null;
        yield return null;
        yield return null;

        inst = GetComponent<AbilitySystem.AS_Ability_Instance_Base>();

        if (!inst) Destroy(this);
        else print("inst acquired");

        inst.Cooldown.OnCooldownUsed += Cooldown_OnCooldownUsed;
    }
}
