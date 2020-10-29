using System.IO;
using UnityEngine;
using Networking.NetworkObjects;
using Networking.Manager;

namespace Networking.NetworkActions
{
    [NetworkActionId(NetworkActionId.RegisterChild)]
    public class RegisterChildAction : INetworkAction
    {
        private RegisterChildArgs args;
        public RegisterChildAction() { }
        public RegisterChildAction(RegisterChildArgs args)
        {
            this.args = args;
        }

        public int Dispatch(int instanceId, INetworkObjectManager networkObjectManager)
        {
            networkObjectManager.TryGetObject(instanceId, out NetworkObject networkObject);
            if (networkObject != null)
            {
                networkObject.RegisterChild(args);
            }
            return instanceId;
        }

        public void Encode(BinaryWriter writer)
        {
            this.BaseEncode(writer);
            writer.Write(args.childId);
            writer.Write(args.childPath);
        }

        public void Decode(BinaryReader reader, INetworkObjectManager manager)
        {
            args = new RegisterChildArgs
            {
                childId = reader.ReadInt32(),
                childPath = reader.ReadString()
            };
        }
    }
}

