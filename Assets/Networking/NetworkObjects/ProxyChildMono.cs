using UnityEngine;
using Networking.Manager;
using static Networking.NetworkObjects.NetworkObject;
using System.Text;
using System.Collections.Generic;
using Networking.Support;

namespace Networking.NetworkObjects
{
    public class ProxyChildMono : MonoBehaviour
    {
        private void Awake()
        {
            var parent = GetComponentInParent<ProxyMono>();
            parent.OnProxySet += OnProxySet;
        }

        private void OnProxySet(Proxy proxy)
        {
            proxy.RegisterChild(gameObject);
        }
    }
}

