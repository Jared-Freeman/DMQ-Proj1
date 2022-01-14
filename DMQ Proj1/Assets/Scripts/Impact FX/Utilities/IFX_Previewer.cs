using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//using UnityEditor;
//[ExecuteInEditMode]

public class IFX_Previewer : MonoBehaviour
{
    public bool PlayEffect;
    public ImpactFX.ImpactEffect PreviewEffect;

    private void Start()
    {
        StartCoroutine(PlayFX());
    }

    IEnumerator PlayFX()
    {
        while(true)
        {
            if (PlayEffect && PreviewEffect != null)
            {
                PreviewEffect.SpawnImpactEffect(null, transform.position);
                yield return new WaitForSeconds(PreviewEffect.Options.Duration);
            }
            else yield return null;
        }
    }


    //private void OnDrawGizmos()
    //{
    //    Handles.BeginGUI();

    //    Rect rect = new Rect(10, 10, 200, 50);

    //    GUI.Button(rect, "Preview Impact FX");

    //    if (Event.current.type == EventType.MouseDown)
    //    {
    //        Debug.Log("press"); //why not :(
    //        StopAllCoroutines();
    //        StartCoroutine(PlayFX());
    //    }

    //    Handles.EndGUI();
    //}

    //IEnumerator PlayFX()
    //{
    //    yield return new WaitForSeconds(PreviewEffect.Options.Duration);
    //}
}
