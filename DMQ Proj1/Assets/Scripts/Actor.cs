using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ActorStats))]
public class Actor : MonoBehaviour
{

    #region members

    private static int IdGenerator = 0;
    
    readonly public int ID = IdGenerator++;

    public ActorStats stats;

    #endregion
    

    // Start is called before the first frame update
    void Start()
    {
         
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
