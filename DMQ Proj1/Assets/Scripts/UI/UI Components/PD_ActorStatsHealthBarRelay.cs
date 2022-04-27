using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PD_ActorStatsHealthBarRelay : MonoBehaviour
{
    protected HealthBar _HB;
    public ActorStats _Stats;

    [Range(.01f, 1f)]
    public float PercentageOfFullCircle = .25f;

    private float _PercentageOfFullCircle_reciprocal;


    void Awake()
    {
        _HB = GetComponent<HealthBar>();
        if (_HB == null) Destroy(this);

        _PercentageOfFullCircle_reciprocal = 1 / PercentageOfFullCircle;
    }

    void Update()
    {
        if(_Stats != null)
        {
            _HB.curHealth = _Stats.HpCurrent;

            _HB.maxHealth = _Stats.Preset.Data.HP.Default.Max;
            _HB.maxHealthTotal = _Stats.Preset.Data.HP.Default.Max * _PercentageOfFullCircle_reciprocal;
        }
    }
}
