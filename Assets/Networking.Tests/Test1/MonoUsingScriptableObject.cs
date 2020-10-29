using System.Collections.Generic;
using UnityEngine;
using Networking.Manager;

namespace Networking.Tests
{
    public class MonoUsingScriptableObject : MonoBehaviour
    {
        private void OnValidate()
        {
            Debug.Assert(!manager || typeof(INetworkObjectManager).IsAssignableFrom(manager.GetType()));
        }
        public ScriptableObject manager;
    }
}

