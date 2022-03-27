using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public static class Testing
    {
        /// <summary>
        /// Validates whether <paramref name="reference"/> is non-null, and logs an error upon failure.
        /// </summary>
        /// <param name="reference">The object reference to validate.</param>
        /// <returns>True, if the reference is non-null</returns>
        /// <remarks>For use in <see cref="MonoBehaviour"/> Awake() object validations</remarks>
        public static bool ReferenceIsValid(object reference)
        {
            if(reference != null)
            {
                return true;
            }
            Debug.LogError(reference.ToString() + " is null! Was it initialized correctly?");
            return false;
        }
    }
}