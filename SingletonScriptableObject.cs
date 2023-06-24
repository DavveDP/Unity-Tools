using UnityEngine;

namespace DavveDP.Tools
{
    /// <summary>
    /// Uses an instance of the scriptableobject of type T in the Resources folder as a singleton and exposes it. Useful for databases of static data. Use with caution, especially for large projects.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SingletonScriptableObject<T> : ScriptableObject where T : ScriptableObject
    {
        protected static T instance;
        public static T Instance
        {
            get
            {
                instance = instance != null ? instance : Resources.FindObjectsOfTypeAll<T>()[0];
                if (instance == null)
                    Debug.LogError("SingletonScriptableObject<" + nameof(T) + "> instance not found.");
                return instance;
            }
        }
    }
}