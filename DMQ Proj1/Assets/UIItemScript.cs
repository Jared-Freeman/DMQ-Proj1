using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIItemScript : MonoBehaviour
{
    public GameObject item;
    public Texture2D testTexture;
    public new SpriteRenderer renderer;
    public int itemCount = 2;
    public bool onCoolDown = false;
    public MaterialPropertyBlock matProperties;


    void Awake()
    {
        item = this.gameObject;
        renderer = GetComponent<SpriteRenderer>();
        if (renderer == null) Destroy(this);
        matProperties = new MaterialPropertyBlock();
    }
    // Start is called before the first frame update
    void Start()
    {
        //renderer = GetComponent<Renderer>();
        useItem(4.0f);
        changeItem(testTexture);

    }

    void update()
    {
        renderer.SetPropertyBlock(matProperties);
    }

    public void changeItem(Texture2D tex)
    {
        renderer.material.SetTexture("_FlaskImage",tex);
    }

    public void updateItemCount()
    {
        itemCount++;
    }

    public void useItem(float coolDownTime)
    {
        Debug.Log("got here too");
        if(itemCount > 0 && !onCoolDown)
        {
            Debug.Log("got here");
            onCoolDown = true;
            matProperties.SetFloat("_Timer", 1.0f);
            StartCoroutine(itemCoolDown(coolDownTime));
            itemCount--;
            if(itemCount == 0)
            {
                changeItem(testTexture);
            }
            onCoolDown = false;
        }
    }

    //Coroutine to control cooldown on texture
    private IEnumerator itemCoolDown(float itemCoolDownTime)
    {
        Debug.Log("This worked");
        float totalCoolDownTime = itemCoolDownTime;
        while (itemCoolDownTime > 0)
        {
            Debug.Log("Counting Down");
            itemCoolDownTime -= Time.deltaTime;
            matProperties.SetFloat("_Timer", itemCoolDownTime / totalCoolDownTime);
            renderer.SetPropertyBlock(matProperties);
            yield return null;
        }
        Debug.Log("Finished");
    }
}
