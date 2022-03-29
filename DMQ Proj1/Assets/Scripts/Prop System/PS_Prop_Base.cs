using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AbilitySystem;

namespace PropSystem
{

    /// <summary>
    /// Base class for Props. 
    /// </summary>
    /// <remarks>This component gives prop items additional functionality such as Abilities upon collision.</remarks>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(AbilitySystem.AS_ExecuteAbilityOnPhysicCollision))]
    public class PS_Prop_Base : MonoBehaviour
    {
        #region Members

        protected AS_ExecuteAbilityOnPhysicCollision _CollisionInvoker;
        protected Rigidbody _RB;

        #endregion

        #region Initialization

        void Awake()
        {
            _CollisionInvoker = GetComponent<AS_ExecuteAbilityOnPhysicCollision>();
            if (!Utils.Testing.ReferenceIsValid(_CollisionInvoker)) Destroy(this);

            _RB = GetComponent<Rigidbody>();
            if (!Utils.Testing.ReferenceIsValid(_RB)) Destroy(this);
        }

        #endregion
    }
}