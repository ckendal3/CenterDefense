using System;
using UnityEngine;

namespace WDIB.Utilities
{
    /// <summary>
    /// Abstract class for making reload-proof singletons out of ScriptableObjects
    /// Returns the asset created on the editor, or null if there is none
    /// Based on https://www.youtube.com/watch?v=VBA1QCoEAX4
    /// Taken from Installation 01 with permission(!)
    /// </summary>
    /// <typeparam name="T">Singleton type</typeparam>

    public abstract class SingletonScriptableObject<T> : ScriptableObject where T : ScriptableObject
    {
        static T instance = null;
        public static T Instance
        {
            get
            {
                Type typeParameterType = typeof(T);
                if (!instance)
                    instance = (T)Resources.Load(typeParameterType.Name);
                return instance;
            }
        }
    }
}
