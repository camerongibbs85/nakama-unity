using Networking.Manager;
using System;
using UnityEngine;

namespace Networking.NetworkObjects
{
    public partial class NetworkObject
    {

        public const int NullInstanceId = int.MinValue;
        private readonly GameObject gameObject;
        private readonly int instanceId;
        private readonly Children children;
        private NetworkObject(int instanceId, GameObject gameObject)
        {
            this.gameObject = gameObject;
            this.instanceId = instanceId;
            this.children = new Children(this);
        }

        public void SetParent(SetParentArgs args, INetworkObjectManager networkObjectManager)
        {
            networkObjectManager.TryGetObject(args.parentId, out NetworkObject parent);
            if (args.parentId == NullInstanceId)
            {
                gameObject.transform.SetParent(null, args.worldPositionStays);
            }
            else if (parent != null)
            {
                parent.PerformArgsAction(args, parentObject => gameObject.transform.SetParent(parentObject.transform, args.worldPositionStays));
            }
        }

        public void SetLocalPosition(PositionArgs args)
        {
            PerformArgsAction(args, gameObject => gameObject.transform.localPosition = args.position);
        }

        public int Destroy(DestroyArgs args, INetworkObjectManager networkObjectManager)
        {
            if(args.ChildId == NullInstanceId) networkObjectManager.DestroyNetworkObject(instanceId);
            PerformArgsAction(args, gameObject => UnityEngine.Object.Destroy(gameObject));
            return instanceId;
        }

        public void RegisterChild(RegisterChildArgs args)
        {
            children.RegisterChild(args.childId, args.childPath);
        }

        private void PerformArgsAction(ActionArgs args, Action<GameObject> action)
        {
            GameObject actionGameObject = children.GetChild(args.ChildId) ?? gameObject;
            action(actionGameObject);
        }
    }
}

