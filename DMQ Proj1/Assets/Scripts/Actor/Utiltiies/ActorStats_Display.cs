using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ActorSystem;

[RequireComponent(typeof(ActorStats))]
public class ActorStats_Display : MonoBehaviour
{

    #region members
    [Header("Properties")]
    public Vector3 Offset = new Vector3(0, 3, 0);
    public float FontSize = 4;
       
    [Header("Can be left empty")]
    public GameObject OptionalParent;
    
    [Header("Auto-Attached Refs")]
    public ActorStats AttachedStats;
    public Text_Bubble StatsText;

    //private members
    private GameObject OffsetTransformHost;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        AttachedStats = GetComponent<ActorStats>();
        if (AttachedStats == null) Destroy(this);

        OffsetTransformHost = new GameObject();
        OffsetTransformHost.name = "ActorStats Display Host";
        if (OptionalParent != null)
        {
            OffsetTransformHost.transform.SetParent(OptionalParent.transform);
        }
        else
        {
            OffsetTransformHost.transform.SetParent(gameObject.transform);
        }
        OffsetTransformHost.transform.localPosition = new Vector3(0, 0, 0);
        OffsetTransformHost.transform.localPosition += Offset;

        StatsText = Text_Bubble.CreateTextBubble("test text", OffsetTransformHost);
        StatsText.text_mesh_pro.fontSize = FontSize;
        StatsText.text_mesh_pro.color = Color.red;
        UpdateDisplayText();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateDisplayText();
    }

    void UpdateDisplayText()
    {
        StatsText.text_mesh_pro.SetText("HP: " + AttachedStats.HpCurrent);
    }
}
