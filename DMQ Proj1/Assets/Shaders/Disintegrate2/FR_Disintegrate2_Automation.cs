using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// For use with disintegrate2 shader implemented by Jared Freeman
/// </summary>
public class FR_Disintegrate2_Automation : MonoBehaviour
{
    [SerializeField] private Renderer _Renderer;

    public int _DissolveMaterialIndex = 0;
    [Header("optional")]
    public Collider _ColliderBoundsReference;
    /// <summary>
    /// Additional tolerance beyond the collider bounds limit to better encapsulate the object
    /// </summary>
    public float _ColliderBounds_Add = .5f;

    [Min(0f)]
    public float _TimeDissolveStart = 5f;
    [Min(0f)]
    public float _TimeDissolveEnd = 10f;

    MaterialPropertyBlock _MatBlock;

    [SerializeField] private float yCurrent;

    void Awake()
    {
        _Renderer = GetComponent<Renderer>();
        if (!Utils.Testing.ReferenceIsValid(_Renderer)) Destroy(this);


        _MatBlock = new MaterialPropertyBlock();
        //_Renderer.GetPropertyBlock(_MatBlock, _DissolveMaterialIndex);
    }

    void Start()
    {
        //clamp end
        _TimeDissolveEnd = Mathf.Clamp(_TimeDissolveEnd, _TimeDissolveStart, Mathf.Infinity);

        _MatBlock.SetFloat("_CutoffHeight", Mathf.Infinity);
        _Renderer.SetPropertyBlock(_MatBlock);

        StartCoroutine(I_ContinueDissolve(_TimeDissolveStart, _TimeDissolveEnd));
    }

    protected IEnumerator I_ContinueDissolve(float startDelay, float endTime)
    {
        float dissolveDuration = endTime - startDelay;
        float startTime = Time.time;
        float elapsedTime = 0f;
        while (elapsedTime < startDelay)
        {
            elapsedTime = Time.time - startTime;
            yield return null;
        }
        while (elapsedTime < endTime)
        {
            elapsedTime = Time.time - startTime;

            if(_ColliderBoundsReference != null)
            {
                yCurrent = _ColliderBoundsReference.bounds.center.y + _ColliderBounds_Add + _ColliderBoundsReference.bounds.extents.y;
            }
            else
            {
                yCurrent = _Renderer.bounds.center.y + _Renderer.bounds.extents.y;
            }
            //-y current is lowest point in AABB, 2*yCurrent is the height. We take a percentage of height based on time to parameterize the dissolve y
            _MatBlock.SetFloat("_CutoffHeight", -yCurrent + 2 * yCurrent * (1 - elapsedTime / dissolveDuration));
            _Renderer.SetPropertyBlock(_MatBlock);

            yield return null;
        }


        yCurrent = Mathf.NegativeInfinity;
        _MatBlock.SetFloat("_CutoffHeight", yCurrent);
        _Renderer.SetPropertyBlock(_MatBlock);

    }
}
