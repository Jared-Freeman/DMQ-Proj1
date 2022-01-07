using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public AllPossibleWeapons allPossibleWapons;

    public WeaponHolder[] equipWep = new WeaponHolder[2];
    //0 is right hand, 1 is left hand
    public Transform[] handPostions = new Transform[2];
    public int currentEquipNumber = -1;
    public int changeToNumber = -1;

    public void WeaponSwitch()
    {
        int equipNum = -1;
        if (currentEquipNumber == changeToNumber)
            equipNum = -1;
        else
            equipNum = changeToNumber;

        if (currentEquipNumber >= 0)
        {
            for(int i = 0; i < handPostions.Length; i++)
            {
                if (handPostions[i].childCount > 0)
                {
                    for (int j = handPostions[i].childCount-1; j >= 0; j--)
                    {
                        Destroy(handPostions[i].GetChild(j).gameObject);
                    }
                }
            }
        }
        if (equipNum >= 0)
        {
            for (int i = 0; i < handPostions.Length; i++)
            {
                if (handPostions[i] != null)
                {
                    //number of models fills number of hand positions
                    if (allPossibleWapons.weaponList[equipWep[equipNum].index].model.Length > i)
                    {
                        //something there
                        if (allPossibleWapons.weaponList[equipWep[equipNum].index].model[i] != null)
                        {
                            GameObject w = Instantiate(allPossibleWapons.weaponList[equipWep[equipNum].index].model[i], handPostions[i].position, handPostions[i].rotation);
                            w.transform.parent = handPostions[i];
                        }
                    }
                }
            }
        }

        currentEquipNumber = equipNum;
    }
}

[System.Serializable]
public class WeaponHolder
{
    public int index;
    public string name;
}

[SerializeField]
public enum SpecialAction
{None, Dash, Guard }