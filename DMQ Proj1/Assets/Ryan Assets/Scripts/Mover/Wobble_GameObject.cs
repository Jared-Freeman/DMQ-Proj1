using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wobble_GameObject : MonoBehaviour
{
    [Range(.001f,1f)]
    public float anti_spiciness = 1;
    [Range(0f,100f)]
    public float leash_range = 1;
    [Range(0f, 100f)]
    public float max_velocity = 1;

    [SerializeField]
    private float t_param;
    [SerializeField]
    private Vector3 root_position;
    [SerializeField]
    private Vector3 cur_velocity;

    // Start is called before the first frame update
    void Start()
    {
        root_position = transform.position;
        cur_velocity = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        float cooking_temperature = 1 / anti_spiciness;
        cur_velocity += new Vector3(Random.Range(-cooking_temperature, cooking_temperature), Random.Range(-cooking_temperature, cooking_temperature), Random.Range(-cooking_temperature, cooking_temperature));

        //clamp
        if (cur_velocity.sqrMagnitude > max_velocity * max_velocity)
        {
            cur_velocity = cur_velocity.normalized * max_velocity;
        }

        Vector3 new_position = transform.position + cur_velocity * Time.deltaTime;

        //clamp
        float distance = (root_position - new_position).magnitude;
        distance = Mathf.Clamp(distance, 0, leash_range);
        /*
        //clamp
        if (new_position.sqrMagnitude > leash_range * leash_range)
        {
            new_position = new_position.normalized * leash_range;
        }
        */
        t_param = Freeman_Utilities.MapValueFromRangeToRange(distance, 0, leash_range, 0, 1);
        t_param = 1 - t_param;

        transform.position = Vector3.Lerp(transform.position, new_position, t_param);
    }
}
