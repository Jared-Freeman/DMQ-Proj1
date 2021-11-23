using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Follow_Target : MonoBehaviour
{
    public bool flag_use_initial_offset;
    public GameObject target;

    [SerializeField] private Vector3 initial_offset;
    public Vector3 offset_vector;

    [Range(1, 30)]
    public int num_smoothframes = 5;

    private Queue<Vector3> position_queue = new Queue<Vector3>();
    
    private void Start()
    {
        
        if (target == null)
        {
            Destroy(this);
        }

        //init buffer
        for(int i=0; i < num_smoothframes; i++)
        {
            position_queue.Enqueue(transform.position);
        }

        initial_offset = gameObject.transform.position - target.transform.position;
    }

    private void Update()
    {
        if(flag_use_initial_offset)
        {
            position_queue.Enqueue(target.transform.position + initial_offset);
        }
        else
        {
            position_queue.Enqueue(target.transform.position + offset_vector);
        }
        while(position_queue.Count > num_smoothframes)
        {
            position_queue.Dequeue();
        }

        //get avg
        Vector3 next_position = Vector3.zero;
        foreach (Vector3 vec in position_queue)
        {
            next_position += vec;
        }
        next_position /= position_queue.Count;

        //assign new pos
        gameObject.transform.position = next_position;
    }
}
