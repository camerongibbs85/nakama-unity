using System.Collections.Generic;
using UnityEngine;

namespace Networking.Support
{
    [System.Serializable]
    public class ScriptableInterfaceList<T> where T : ScriptableObject
    {
        public ScriptableInterfaceList()
        {
            items = new List<T>();
        }
        public ScriptableInterfaceList(int capacity)
        {
            items = new List<T>(capacity);
        }
        public ScriptableInterfaceList(IEnumerable<T> collection)
        {
            items = new List<T>(collection);
        }
        public List<T> items;
    }

    [System.Serializable]
    public class ScriptableInterfaceList : ScriptableInterfaceList<ScriptableObject> { }
}

