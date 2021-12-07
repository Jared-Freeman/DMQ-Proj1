using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DevStepMaker : MonoBehaviour
{
    public List<AudioSource> stepSounds;
    public List<AudioSource> fallSounds;
    public Transform transformReference;
    int index = 0;

    public float StepDistance = .25f;
    [SerializeField]
    private Vector3 LastStepLocation;
    [SerializeField]
    private Vector3 CurStepLocation;

    // Start is called before the first frame update
    void Start()
    {
        LastStepLocation = transformReference.position;
    }

    private void Update()
    {
        CurStepLocation = transformReference.position;
        if ((CurStepLocation - LastStepLocation).sqrMagnitude > StepDistance * StepDistance)
        {
            LastStepLocation = CurStepLocation;
            DoStep();
        }
    }

    public void DoFall()
    {
        index += Random.Range(1, 3);
        index %= fallSounds.Count;
        AudioSource.PlayClipAtPoint(fallSounds[index].clip, CurStepLocation);
    }

    public void DoStep()
    {
        Debug.Log("Dostep");
        index += Random.Range(1,3);
        index %= stepSounds.Count;
        AudioSource.PlayClipAtPoint(stepSounds[index].clip, CurStepLocation);
    }
}
