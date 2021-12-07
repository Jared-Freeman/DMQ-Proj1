using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoFaceUpwardGlobally : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Vector3 rot = Vector3.up;
        gameObject.transform.rotation.eulerAngles.Set(rot.x, rot.y, rot.z);
    }

}
