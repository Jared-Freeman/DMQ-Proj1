using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ActorStats))]
public class ActorStats_Display : MonoBehaviour
{

    #region members
    public ActorStats AttachedStats;
    public Text_Bubble StatsText;
    public GameObject OffsetTransformHost;

    public Vector3 Offset = new Vector3(0, 5, 0);
    public float FontSize = 4;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        AttachedStats = GetComponent<ActorStats>();
        if (AttachedStats == null) Destroy(this);

        OffsetTransformHost = new GameObject();
        OffsetTransformHost.name = "ActorStats Display Host";
        OffsetTransformHost.transform.SetParent(gameObject.transform);
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
