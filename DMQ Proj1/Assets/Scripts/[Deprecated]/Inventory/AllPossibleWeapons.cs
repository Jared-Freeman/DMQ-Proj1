using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "All Possible Weapons", menuName = "Inventory/All Possible Weapons")]
public class AllPossibleWeapons : ScriptableObject
{
    public List<Weapon> weaponList = new List<Weapon>();
}
[System.Serializable]
public class Weapon
{
    public string name;
    public int weaponAttack = 0;
    public GameObject[] model = new GameObject[2];
    public SpecialAction specialAction;
    public int weaponAnimType = 0;
}