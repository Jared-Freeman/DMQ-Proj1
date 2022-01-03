using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FR_PerlineNoiseTest : MonoBehaviour
{
    public GameObject Instance;

    //yes there is a better way to do this but whatever
    public float Spacing = 1f;
    private float SpacingBuffer = 1f;
    
    public float HeightAmplifier = 1f;
    private float HeightAmplifierBuffer = 0f;

    public Vector2Int Dimensions = Vector2Int.zero;
    private Vector2Int DimensionsBuffer;

    [Min(.0001f)]
    public float Resolution = 1f;
    private float ResolutionBuffer = 1f;

    private List<GameObject> InstanceList;
    
    private void Awake()
    {
        InstanceList = new List<GameObject>();

        DimensionsBuffer = Dimensions;
        HeightAmplifierBuffer = HeightAmplifier;
        SpacingBuffer = Spacing;
        ResolutionBuffer = Resolution;
    }

    private void Start()
    {
        ComputeNoiseGrid();
    }
    
    void Update()
    {
        if(DimensionsBuffer != Dimensions || HeightAmplifier != HeightAmplifierBuffer || Spacing != SpacingBuffer || ResolutionBuffer != Resolution)
        {
            ComputeNoiseGrid();

            DimensionsBuffer = Dimensions;
            HeightAmplifierBuffer = HeightAmplifier;
            SpacingBuffer = Spacing;
            ResolutionBuffer = Resolution;
        }
    }

    void ComputeNoiseGrid()
    {
        ClearInstanceList();

        GameObject CurInstance;

        for (int i = 0; i < Dimensions.x; i++)
        {
            for (int j = 0; j < Dimensions.y; j++)
            {
                CurInstance = Instantiate(Instance);
                InstanceList.Add(CurInstance);

                CurInstance.transform.position = new Vector3(
                    i * Spacing,
                    Mathf.Clamp(Mathf.PerlinNoise((float)i / Resolution, (float)j / Resolution), 0, 1) * HeightAmplifier,
                    j * Spacing
                    );

                CurInstance.transform.parent = gameObject.transform;
            }
        }
    }

    private void ClearInstanceList()
    {
        foreach(GameObject GO in InstanceList)
        {
            GameObject.Destroy(GO);
        }

        InstanceList.Clear();
    }
}
