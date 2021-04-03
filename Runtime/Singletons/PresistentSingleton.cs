using UnityEngine;

namespace Agraris.Tools.Core
{
    /// <summary>
    /// Persistent singleton.
    /// </summary>
    public class PersistentSingleton<T> : MonoBehaviour where T : Component
    {
        protected static T _instance;

        /// <summary>
        /// The singleton instance.
        /// </summary>
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    _instance = obj.AddComponent<T>();
                }
                return _instance;
            }
        }

        /// <summary>
        /// On awake, we check if there's already a copy of the object in the scene. If there's one, we destroy it.
        /// </summary>
        protected virtual void Awake()
        {
            if (_instance != null)
            {
                //there is already an instance:
                Destroy(gameObject);
                return;
            }

            //If I am the first instance, make me immortal
            _instance = this as T;
            DontDestroyOnLoad(transform.gameObject);
        }
    }
}
