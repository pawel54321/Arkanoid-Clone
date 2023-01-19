using UnityEngine;

namespace Utility
{
    public class Singleton<T> : MonoBehaviour where T : Component
    {
        #region Field
        private static T _instance;
        #endregion

        #region Propertie
        public static T GetInstance() => _instance;
        #endregion

        #region Method
        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(this);
            }
            else
                Destroy(gameObject);
        }
        #endregion
    }
}