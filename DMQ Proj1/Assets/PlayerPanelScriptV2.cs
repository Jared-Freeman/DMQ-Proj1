using CSEventArgs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPanelScriptV2 : MonoBehaviour
{
    public class UIPlayerInfo
    {
        public Actor_Player _Player;
        public Inventory_Player _Inventory;
        public int EquipedWeaponIndex = 0;

        public UIPlayerInfo(GameObject p)
        {
            _Player = p.GetComponent<Actor_Player>();
            _Inventory = p.GetComponent<Inventory_Player>();
        }
    }

    public UIPlayerInfo player;

    public List<Image> ListWeaponSlots = new List<Image>();
    public List<GameObject> ListWeaponSlotOneTypes = new List<GameObject>();
    public List<GameObject> ListWeaponSlotTwoTypes = new List<GameObject>();
    public List<Text> ListAbilityName = new List<Text>();
    public Text CharacterClass;
    


    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnEnable()
    {
        setCharacterClassName(player._Player.Class.ClassName);
        switchActiveWeapon(player._Inventory.GetInfo().EquippedWeaponIndex);
    }
    // Update is called once per frame
    void Update()
    {
        if(player._Inventory.getWeaponSlots()[0].Weapon != null && player._Inventory.getWeaponSlots()[1].Weapon != null)
        {
            setActiveWeaponOneIcon(getWeaponIndex(player._Inventory.getWeaponSlots()[0]));
            setActiveWeaponTwoIcon(getWeaponIndex(player._Inventory.getWeaponSlots()[1]));
        }
        switchActiveWeapon(player._Inventory.GetInfo().EquippedWeaponIndex);
    }

    public void setActiveWeaponOneIcon(int i)
    {
        if(ListWeaponSlotOneTypes[1] == null ||
            ListWeaponSlotOneTypes[2] == null ||
            ListWeaponSlotOneTypes[3] == null ||
            ListWeaponSlotOneTypes[4] == null)
        {
            Debug.LogError("Weapon slot one icons not set");
            return;
        }

        for(int j = 0;j < ListWeaponSlotOneTypes.Count;j++)
        {
            if(j == i)
            {
                ListWeaponSlotOneTypes[j].SetActive(true);
            }
            else
            {
                ListWeaponSlotOneTypes[j].SetActive(false);    
            }
        }
    }

    public void setActiveWeaponTwoIcon(int i)
    {
        if(ListWeaponSlotTwoTypes[1] == null ||
            ListWeaponSlotTwoTypes[2] == null ||
            ListWeaponSlotTwoTypes[3] == null ||
            ListWeaponSlotTwoTypes[4] == null)
        {
            Debug.LogError("Weapon slot two icons not set");
            return;
        }

        for(int j = 0;j<ListWeaponSlotTwoTypes.Count; j++)
        {
            if(i == j)
            {
                ListWeaponSlotTwoTypes[j].SetActive(true);
            }
            else
            {
                ListWeaponSlotTwoTypes[j].SetActive(false);
            }
        }
    }

    public void switchActiveWeapon(int i)
    {
        if(ListWeaponSlots[0] == null || ListWeaponSlots[1] == null)
        {
            Debug.LogError("Weapon Slot not set");
            return;
        }
        if(i == 0)
        {
            ListWeaponSlots[0].color = Color.yellow;
            ListWeaponSlots[1].color = Color.white;
        }
        else if(i == 1)
        {
            ListWeaponSlots[0].color = Color.white;
            ListWeaponSlots[1].color = Color.yellow;
        }
        else
        {
            ListWeaponSlots[0].color= Color.white;
            ListWeaponSlots[1].color = Color.white;
        }
    }

    public void setCharacterClassName(string newClass)
    {
        if(CharacterClass != null)
        {
            CharacterClass.text = newClass;
        }
        else
        {
            Debug.LogError("Character class text not set");
        }
    }

    public UIPlayerInfo GetPlayer()
    {
        return player;
    }

    public void SetPlayer(UIPlayerInfo value)
    {
        player = value;
    }

    public int getWeaponIndex(Inventory_WeaponSlot weapon)
    {
        int index = 0;
        if(weapon.Weapon.Preset.BaseOptions.name != null)
        {
            string var = weapon.Weapon.Preset.BaseOptions.name;
            switch (var)
            {
                case "Flintlock":
                    {
                        index = 1;
                        break;
                    }
                case "Sword":
                    {
                        index = 2;
                        break;
                    }
                case "Axe":
                    {
                        index = 3;
                        break;
                    }
                case "Shield":
                    {
                        index = 4;
                        break;
                    }
                case "Bow":
                    {
                        index = 5;
                        break;
                    }
                default:
                    break;
            }
        }
        return index;
    }
}
