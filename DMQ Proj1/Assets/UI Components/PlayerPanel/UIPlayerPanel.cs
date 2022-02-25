using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;



public class UIPlayerPanel : MonoBehaviour
{
    protected SpriteRenderer _Renderer;

    public GameObject activePanel;
    public GameObject inactivePanel;

    public GameObject weaponOne;
    public GameObject weaponTwo;
    public int activeWeapon = 0;

    public GameObject[] items;

    public MaterialPropertyBlock[] matProperties;

    //public TextMeshProUGUI characterClassText;
    public TMP_Text characterClassText;
    public float time = 10.0f;

    void awake()
    {
        _Renderer = GetComponent<SpriteRenderer>();
        if (_Renderer == null) Destroy(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        characterClassText = this.GetComponentInChildren<TMP_Text>();
        setCharacterClassName("MINE");
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < 2; i++)
        {
            _Renderer.SetPropertyBlock(matProperties[i]);
        }
    }

    #region functions
    //function for setting character class name in UI
    public void setCharacterClassName(string newClass)
    {
        characterClassText.SetText(newClass);
    }

    //Set Active weapon to front of UI
    public void swapWeapon()
    {
        if(weaponOne != null && weaponTwo != null)
        {
            if(activeWeapon == 0)
            {
                activeWeapon = 1;
                weaponOne.transform.position = new Vector3(-100,-2,-2);
                weaponTwo.transform.position = new Vector3(-50,-1,-4);

            }
            else
            {
                activeWeapon = 0;
                weaponOne.transform.position = new Vector3(-100,-2,-4);
                weaponTwo.transform.position= new Vector3(-50,-1,-2);
            }
        }
    }

    //Change weapon texture, used to signal different weapons
    public void changeWeapon(Texture tex)
    {
        Renderer m_renderer = null;
        if(weaponOne != null && weaponTwo != null)
        {
            if(activeWeapon == 0)
            {
                m_renderer = weaponOne.GetComponent<Renderer>();
                m_renderer.material.SetTexture("_WeaponTex", tex);
            }
            else
            {
                m_renderer=weaponTwo.GetComponent<Renderer>();
                m_renderer.material.SetTexture("_WeaponTex", tex);
            }
        }
    }

    #endregion

    #region depricated
    /*
//Use item, decrements number of items, starts cooldown
public void useItem(int index)
{
    float count = matProperties[index].GetFloat("_ItemCount");
    if (count > 0)
    {
        count--;
        matProperties[index].SetFloat("_ItemCount", count);
        //StartCoroutine(itemCoolDown(time, 0));

        if (count == 0)
        {
            items[index].SetActive(false);
        }
    }
}

//Change item texture, used to signal different items
public void addItem(Texture tex, int index)
{
    Renderer m_renderer = null;
    m_renderer = items[index].GetComponent<Renderer>();
    if (m_renderer != null)
    {
        m_renderer.material.SetTexture("_FlaskImage", tex);
        items[index].SetActive(true);
        matProperties[index].SetFloat("", 1);
    }
}
*/
    #endregion
}
