using UnityEngine;
using Networking.Manager;
using static Networking.NetworkObjects.NetworkObject;
using System;

namespace Networking.NetworkObjects
{

    public class ProxyMono : MonoBehaviour
    {
        
        private event Action<Proxy> onProxySet;
        public event Action<Proxy> OnProxySet
        {
            add
            {
                onProxySet += value;
                if(proxy != null) value(proxy);
            }
            remove => onProxySet -= value;
        }
        private Proxy proxy;
        public Proxy Proxy
        {
            get
            {
                return proxy;
            }
            set
            {
                proxy = value;
                Notify(proxy);
            }
        }

        private void Notify(Proxy proxy)
        {
            if (onProxySet != null)
            {
                Action<Proxy> evt = new Action<Proxy>(onProxySet);
                evt(proxy);
            }
        }

        public Proxy AddToManager(INetworkObjectManager manager)
        {
            Proxy = manager.Add(this);
            return Proxy;
        }
    }
}

