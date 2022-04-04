using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop_Point_Parent : MonoBehaviour
{
    public int index;
    public int totalCollections = 3;// totalCollections represents the max amount of collections to be chosen from in Procedural Props Script
    // Start is called before the first frame update
    void Start()
    {
        index = Random.Range(0, totalCollections);
    }
}
