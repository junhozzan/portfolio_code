using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static T m_instance;
    public static T Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindAnyObjectByType<T>();

                if (m_instance == null)
                {
                    m_instance = new GameObject(typeof(T).Name).AddComponent<T>();
                }
            }

            return m_instance;
        }
    }

    private Transform m_transform = null;
    public new Transform transform
    {
        get
        {
            if (m_transform == null)
            {
                m_transform = base.transform;
            }

            return m_transform;
        }

    }

    protected virtual void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this as T;
        }

        DontDestroyOnLoad(m_instance.transform.root);
    }

#if USE_DEBUG
    protected const bool _DEBUG = true;
#else
    protected const bool _DEBUG = false;
#endif

}
