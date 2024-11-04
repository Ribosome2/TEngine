using UnityEngine;

namespace Framework.Utilities
{
    public class MonoBehaviourSingleton<T> : MonoBehaviour where T : Component
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType(typeof(T)) as T;
                    if (_instance == null)
                    {
                        GameObject obj = new GameObject(typeof(T).FullName);

                        if (Application.isPlaying)
                            DontDestroyOnLoad(obj);

                        _instance = obj.AddComponent<T>();
                    }
                }
                return _instance;
            }
        }

        public static void DestroySelf()
        {
            _instance = FindObjectOfType(typeof(T)) as T;
            if (_instance != null)
                DestroyImmediate(_instance.gameObject);
            _instance = null;
        }
    }
}
