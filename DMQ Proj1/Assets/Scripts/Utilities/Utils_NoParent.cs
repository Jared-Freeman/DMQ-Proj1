using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    /// <summary>
    /// Detatches this GameObject from all parents
    /// </summary>
    public class Utils_NoParent : MonoBehaviour
    {
        void Start()
        {
            transform.parent = null;
        }
    }

}