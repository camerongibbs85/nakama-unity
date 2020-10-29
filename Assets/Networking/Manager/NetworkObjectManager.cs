using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System;
using Networking.NetworkObjects;
using static Networking.NetworkObjects.NetworkObject;
using Networking.NetworkActions;
using Networking.Support;
using Networking.ObjectCreator;

namespace Networking.Manager
{
    public partial class NetworkObjectManager : ScriptableObject, INetworkObjectManager
    {
        private readonly Dictionary<int, NetworkObject> networkObjects;
        private INetworkObjectHandler handler;

        [SerializeField]
        [ScriptableInterfaceList(typeof(INetworkedCreator))]
        private ScriptableInterfaceList networkedCreatorList = new ScriptableInterfaceList();

        private List<INetworkedCreator> INetworkedCreatorList;
        public List<INetworkedCreator> NetworkedCreatorList
        {
            get
            {
                if (INetworkedCreatorList == null)
                {
                    INetworkedCreatorList = networkedCreatorList.items.ConvertAll(item => item as INetworkedCreator);
                }
                return INetworkedCreatorList;
            }
            set
            {
                INetworkedCreatorList = value;
            }
        }

        public Transform ParentTransform { get; set; }

        private T GetCreator<T>() where T : class, INetworkedCreator
        {
            return NetworkedCreatorList.Find(item => item.GetType() == typeof(T)) as T;
        }

        private NetworkObject GetNetworkObject(int instanceId)
        {
            return networkObjects[instanceId];
        }

        public NetworkObjectManager()
        {
            networkObjects = new Dictionary<int, NetworkObject>();
        }
        public INetworkedCreator GetCreator(NetworkCreatorId id)
        {
            return NetworkedCreatorList.Find(item => item.GetId() == id) as INetworkedCreator;
        }

        public Proxy Add(ProxyMono proxyMono)
        {
            // get the proxyMono's GameObject
            var gameObject = proxyMono.gameObject;

            // wrap it with a network object
            var (newId, networkObject, proxy) = Proxy.Create(handler, gameObject);

            // keep track of it
            networkObjects.Add(newId, networkObject);

            return proxy;
        }

        public int UseCreator(NetworkCreatorId creatorId, byte[] bytes, int instanceId)
        {
            // use the appropriate creator to create the object with the encoded bytes
            var gameObject = GetCreator(creatorId).UseBytes(bytes, ParentTransform);

            // wrap it with a network object
            var (newId, networkObject, proxy) = Proxy.Create(handler, gameObject, instanceId);

            // keep track of it
            networkObjects.Add(newId, networkObject);

            return newId;
        }

        public Proxy Create<T>(CreatorFunc<T> creatorFunc)
            where T : class, INetworkedCreator
        {
            var (creatorId, creatorBytes) = creatorFunc(GetCreator<T>());

            // create a network action to record it's creation (that will get serialised into the stream with other object actions)
            var newId = handler.NetworkObjectFunction(NullInstanceId, new NetworkedCreatorAction(creatorId, creatorBytes));

            // return the proxy (so that users can't directly manipulate the object. Instead they use its proxy that will manipulate the object through network actions)
            if (TryGetObject(newId, out NetworkObject networkObject))
            {
                return Proxy.From(handler, networkObject);
            }
            else
            {
                return null;
            }
        }

        public bool TryGetObject(int instanceId, out NetworkObject networkObject)
        {
            return networkObjects.TryGetValue(instanceId, out networkObject);
        }

        public void SetHandler(INetworkObjectHandler handler)
        {
            this.handler = handler;
        }

        public void DestroyNetworkObject(int instanceId)
        {
            if (TryGetObject(instanceId, out NetworkObject networkObject))
            {
                networkObjects.Remove(instanceId);
            }
        }
    }
}

