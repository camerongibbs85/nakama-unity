using UnityEngine;
using Networking.NetworkActions;
using Networking.Manager;
using System;
using Networking.Support;
using static Networking.NetworkObjects.NetworkObject;

namespace Networking.NetworkObjects
{
    public class ActionArgs
    {
        public int ChildId = NetworkObject.NullInstanceId;
    }
    public class PositionArgs : ActionArgs
    {
        public Vector3 position;
    }
    public class DestroyArgs : ActionArgs { }
    public class RegisterChildArgs : ActionArgs
    {
        public int childId;
        public string childPath;
    }
    public class SetParentArgs : ActionArgs
    {
        public int parentId;
        public bool worldPositionStays;
    }

    public partial class NetworkObject
    {
        public partial class Proxy
        {
            static readonly DestroyArgs nullDestroyArgs = new DestroyArgs();
            public static (int instanceId, NetworkObject networkObject, Proxy proxy) Create(INetworkObjectHandler networkObjectHandler, GameObject gameObject, int instanceId = NullInstanceId)
            {
                var newId = instanceId == NullInstanceId ? gameObject.GetInstanceID() : instanceId;
                var networkObject = new NetworkObject(newId, gameObject);
                var proxy = new Proxy(networkObjectHandler, networkObject);
                gameObject.AddComponent<ProxyMono>().Proxy = proxy;
                return (newId, networkObject, proxy);
            }

            public static Proxy From(INetworkObjectHandler networkObjectHandler, NetworkObject networkObject)
            {
                return new Proxy(networkObjectHandler, networkObject);
            }

            private readonly INetworkObjectHandler networkObjectHandler;
            private readonly NetworkObject networkObject;
            private Proxy(INetworkObjectHandler networkObjectHandler, NetworkObject networkObject)
            {
                this.networkObjectHandler = networkObjectHandler;
                this.networkObject = networkObject;
            }

            public void SetLocalPosition(Vector3 position, GameObject child)
            {
                SetLocalPosition(position, child.GetInstanceID());
            }

            public void SetLocalPosition(Vector3 position, int childId = NullInstanceId)
            {
                SetLocalPosition(new PositionArgs{position = position});
            }

            public void SetLocalPosition(PositionArgs args)
            {
                networkObjectHandler.NetworkObjectFunction(networkObject.instanceId, new PositionAction(args));
            }

            public void SetParent(Proxy parent, bool worldPositionStays, GameObject child)
            {
                SetParent(parent, worldPositionStays, child.GetInstanceID());
            }

            public void SetParent(Proxy parent, bool worldPositionStays, int childId = NullInstanceId)
            {
                SetParentArgs args = new SetParentArgs
                {
                    ChildId = childId,
                    parentId = parent.networkObject.instanceId,
                    worldPositionStays = worldPositionStays
                };
                SetParent(args);
            }

            public void SetParent(SetParentArgs args)
            {
                networkObjectHandler.NetworkObjectFunction(networkObject.instanceId, new ParentAction(args));
            }

            public void Destroy(GameObject child)
            {
                Destroy(new DestroyArgs{ ChildId = child.GetInstanceID() } );
            }

            public void Destroy()
            {
                Destroy(nullDestroyArgs);
            }

            public void Destroy(DestroyArgs args)
            {
                networkObjectHandler.NetworkObjectFunction(networkObject.instanceId, new DestroyAction(args));
            }

            public int RegisterChild(string path)
            {
                var child = HierarchyUtils.GetChildFromParentPath(networkObject.gameObject.transform, path).gameObject;
                return RegisterChild(child);
            }

            public int RegisterChild(GameObject child)
            {
                RegisterChildArgs args = new RegisterChildArgs
                {
                    childId = child.GetInstanceID(),
                    childPath = HierarchyUtils.BuildPathToParent(child.transform, networkObject.gameObject.transform)
                };
                RegisterChild(args);
                return args.childId;
            }

            public void RegisterChild(RegisterChildArgs args)
            {
                networkObjectHandler.NetworkObjectFunction(networkObject.instanceId, new RegisterChildAction(args));
            }
        }
    }
}
