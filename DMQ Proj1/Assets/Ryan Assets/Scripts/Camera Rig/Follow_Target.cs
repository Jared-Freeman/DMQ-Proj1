using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Follow_Target : MonoBehaviour
{
    public GameObject target;
    public float offset = 1;
    private void Start()
    {
        
        if (target == null)
        {
            Destroy(this);
        }
    }

    private void Update()
    {
        //assign new pos
        gameObject.transform.position = new Vector3(target.transform.position.x, target.transform.position.y , target.transform.position.z);
    }
}
