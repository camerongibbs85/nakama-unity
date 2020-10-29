using System;
using UnityEngine;
using Networking.NetworkObjects;
using static Networking.NetworkObjects.NetworkObject;
using Networking.ObjectCreator;
using System.Collections.Generic;

namespace Networking.Manager
{
    public class CreatorResult : Tuple<NetworkCreatorId, byte[]>
    {
        public CreatorResult(NetworkCreatorId item1, byte[] item2) : base(item1, item2) {}
        public void Deconstruct(out NetworkCreatorId item1, out byte[] item2)
        {
            item1 = Item1;
            item2 = Item2;
        }
    }
    public delegate CreatorResult CreatorFunc<T>(T creator) where T : class, INetworkedCreator;

    public interface INetworkObjectManager
    {
        Transform ParentTransform { get; set; }

        List<INetworkedCreator> NetworkedCreatorList { get; set; }

        void SetHandler(INetworkObjectHandler handler);

        INetworkedCreator GetCreator(NetworkCreatorId id);

        int UseCreator(NetworkCreatorId creatorId, byte[] bytes, int instanceId);

        bool TryGetObject(int instanceId, out NetworkObject networkObject);

        Proxy Create<T>(CreatorFunc<T> creatorFunc) where T : class, INetworkedCreator;

        Proxy Add(ProxyMono proxyMono);

        void DestroyNetworkObject(int instanceId);
    }
}

