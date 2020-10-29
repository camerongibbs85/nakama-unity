using System.Collections.Generic;
using UnityEngine;
using Networking.Support;
using Networking.Manager;

namespace Networking.Tests
{
    public class Test2Mono : MonoBehaviour
    {
        [ScriptableInterface(typeof(INetworkObjectManager))]
        public ScriptableObject sObject;

        public List<int> listInt = new List<int>();

        [ScriptableInterfaceList(typeof(INetworkObjectManager))]
        public ScriptableInterfaceList scriptableInterfaceList = new ScriptableInterfaceList();
    }
}

