using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EffectTree.Condition
{
    [System.Serializable]
    public class ConditionList
    {
        public List<E_Condition_Base> _Conditions = new List<E_Condition_Base>();

        /// <summary>
        /// Evaluates all Conditions in list and returns boolean result. CAN short circuit so only use this for boolean logic (not assignment, etc.)
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns>True, if all conditions evaluate to true</returns>
        public bool Evaluate(ref EffectContext ctx)
        {
            foreach(var c in _Conditions)
            {
                if (!c.EvaluateCondition(ref ctx)) return false;
            }
            return true;
        }
    }

    /// <summary>
    /// Extend this virtual function to implement cool new boolean logic! Make sure to add an asset menu entry! 
    /// </summary>
    public class E_Condition_Base : ScriptableObject
    {
        public virtual bool EvaluateCondition(ref EffectContext ctx)
        {
            return true;
        }
    }
}