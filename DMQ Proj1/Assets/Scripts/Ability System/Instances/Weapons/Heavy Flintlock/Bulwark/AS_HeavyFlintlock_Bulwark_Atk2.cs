﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem.Weapons.HeavyFlintlock
{
    public class AS_HeavyFlintlock_Bulwark_Atk2 : AS_Ability_Instance_Base
    {
        public override bool ExecuteAbility()
        {
            Debug.Log(ToString());
            return base.ExecuteAbility();
        }
    }

    public class AS_HeavyFlintlock_Bulwark_Atk2_Preset : AS_Ability_Base
    {

    }
}