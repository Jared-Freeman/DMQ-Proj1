using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T m_instance;

    public static bool InstanceExists
    {
        get
        {
            if (m_instance != null) return true;
            return false;
        }
    }

public static T Instance
    {
        get
        {
            if (m_instance != null)
            {

            }
            else
            {
                //find instance if it exists
                m_instance = FindObjectOfType<T>();

                //Create new instance
                if(m_instance == null)
                {
                    GameObject GO = new GameObject();

                    GO.name = typeof(T).Name + " [Singleton]";

                    m_instance = GO.AddComponent<T>();
                }
            }


            return m_instance;
        }

    }

    protected virtual void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this as T;
            if(transform.parent == null)
            {
                Debug.Log("Singleton has no parent. Marking as DontDestroyOnLoad.");
                DontDestroyOnLoad(gameObject);
            }
            else
            { 
                //Debug.Log(transform.parent.ToString()); 
            }
        }
        else
        {
            //Destroy(gameObject); //Hopefully this doesnt create any bugs...
        }
    }
}
