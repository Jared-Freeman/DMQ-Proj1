using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Utils.Compare
{
    /// <summary>
    /// Utility for enabling inspector-driven comparison logic. Useful for SO's. See <see cref="EffectTree.Condition.E_C_CollisionForce"/>.
    /// </summary>
    [System.Serializable]
    public class FloatComparison
    {
        public enum ComparisonTypes{ GreaterThan, GreaterThanOrEqual, LessThan, LessThanOrEqual, EqualTo, NotEqualTo }

        public ComparisonTypes ComparisonType = ComparisonTypes.GreaterThan;

        public bool Compare(float lhs, float rhs)
        {
            switch(ComparisonType)
            {
                case ComparisonTypes.GreaterThan:
                    if (lhs > rhs) return true;
                    break;

                case ComparisonTypes.GreaterThanOrEqual:
                    if (lhs >= rhs) return true;
                    break;

                case ComparisonTypes.LessThan:
                    if (lhs < rhs) return true;
                    break;

                case ComparisonTypes.LessThanOrEqual:
                    if (lhs <= rhs) return true;
                    break;

                case ComparisonTypes.EqualTo:
                    if (lhs == rhs) return true;
                    break;

                case ComparisonTypes.NotEqualTo:
                    if (lhs != rhs) return true;
                    break;


                default:
                    Debug.LogError("No impl found for condition option!");
                    return false;
            }
            
            return false;
        }
    }

}