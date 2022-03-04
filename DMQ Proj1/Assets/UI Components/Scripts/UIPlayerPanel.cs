using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;



public class UIPlayerPanel : MonoBehaviour
{
    public List<SpriteRenderer> ListRenderers = new List<SpriteRenderer>();
    public List<GameObject> ListWeaponSlots = new List<GameObject>();
    public int activeWeaponIndex = 1;
    public CanvasRenderer _Renderer;
    private int frames = 0;

    public GameObject activePanel;
    public GameObject inactivePanel;
    public GameObject[] items;

    public MaterialPropertyBlock matProperties;

    //public TextMeshProUGUI characterClassText;
    public TMP_Text characterClassText;
    public float time = 10.0f;

    void Awake()
    {
        var r = GetComponentsInChildren<SpriteRenderer>();
        foreach(var r2 in r)
        {
            ListRenderers.Add(r2);
        }

        _Renderer = GetComponent<CanvasRenderer>();
        //if (_Renderer == null) Destroy(this);
        activePanel = GameObject.Find("ActivePlayerPanel");
        inactivePanel = GameObject.Find("InactivePlayerPanel");
        ListWeaponSlots.Add(GameObject.Find("WeaponOneBorder"));
        ListWeaponSlots.Add(GameObject.Find("WeaponTwoBorder"));
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        frames++;
        if(frames % 10 == 0)
        {
            foreach(var r in ListRenderers)
            {
                r.SetPropertyBlock(matProperties);
            }
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
        if(activeWeaponIndex <= 0)
        {
            var r = ListWeaponSlots[0].GetComponent<RectTransform>();
            var r2 = ListWeaponSlots[1].GetComponent<RectTransform>();
            r.localPosition = new Vector3(-100, -2, -4);
            r2.localPosition = new Vector3(-50, -1, -2);
        }
        else
        {
            var r = ListWeaponSlots[0].GetComponent<RectTransform>();
            var r2 = ListWeaponSlots[1].GetComponent<RectTransform>();
            r.localPosition = new Vector3(-100, -2, -2);
            r2.localPosition = new Vector3(-50, -1, -4);
        }
    }

    public void activePlayerPanel()
    {
        activePanel.SetActive(true);
        inactivePanel.SetActive(false);
    }

    public void setActiveWeaponIndex(int i)
    {
        activeWeaponIndex = i;
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
*/
    #endregion
}
