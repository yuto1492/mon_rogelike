using System;
using UnityEngine;

namespace Extensions
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    Type t = typeof(T);
                    instance = (T)FindObjectOfType(t);
                }
                return instance;
            }
        }
        virtual protected void Awake ()
        {
            if (this != Instance)
            {
                Destroy(this);
            }
        }
    }
}