using System.IO;
using UnityEngine;
using Networking.NetworkObjects;
using Networking.Manager;

namespace Networking.NetworkActions
{
    [NetworkActionId(NetworkActionId.Postition)]
    public class PositionAction : INetworkAction
    {
        private PositionArgs args;
        public PositionAction() { }
        public PositionAction(PositionArgs args)
        {
            this.args = args;
        }

        public int Dispatch(int instanceId, INetworkObjectManager networkObjectManager)
        {
            networkObjectManager.TryGetObject(instanceId, out NetworkObject networkObject);
            if (networkObject != null)
            {
                networkObject.SetLocalPosition(args);
            }
            return instanceId;
        }

        public void Encode(BinaryWriter writer)
        {
            this.BaseEncode(writer);
            writer.Write(args.ChildId);
            writer.Write(args.position.x);
            writer.Write(args.position.y);
            writer.Write(args.position.z);
        }

        public void Decode(BinaryReader reader, INetworkObjectManager manager)
        {
            args = new PositionArgs
            {
                ChildId = reader.ReadInt32(),
                position = new Vector3
                {
                    x = reader.ReadSingle(),
                    y = reader.ReadSingle(),
                    z = reader.ReadSingle()
                }
            };
        }
    }
}

