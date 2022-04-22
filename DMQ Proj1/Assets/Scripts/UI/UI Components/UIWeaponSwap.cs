using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWeaponSwap : MonoBehaviour
{
    public GameObject weaponOne;
    public GameObject weaponTwo;
    public GameObject rotationPoint;

    // Start is called before the first frame update
    void Start()
    {
        weaponOne = GameObject.Find("WeaponOne");
        weaponTwo = GameObject.Find("WeaponTwo");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            weaponOne.transform.RotateAround(rotationPoint.transform.position, Vector3.up,20 * Time.deltaTime);
        }
    }

}
