using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T m_instance;

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
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
