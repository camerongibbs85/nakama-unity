using System.IO;
using Networking.NetworkObjects;
using Networking.Manager;

namespace Networking.NetworkActions
{
    [NetworkActionId(NetworkActionId.Parent)]
    public class ParentAction : INetworkAction
    {
        private SetParentArgs args;

        public ParentAction() { }

        public ParentAction(SetParentArgs args)
        {
            this.args = args;
        }

        public int Dispatch(int instanceId, INetworkObjectManager networkObjectManager)
        {
            networkObjectManager.TryGetObject(instanceId, out NetworkObject networkObject);
            if (networkObject != null)
            {
                networkObject.SetParent(args, networkObjectManager);
            }
            return instanceId;
        }

        public void Encode(BinaryWriter writer)
        {
            this.BaseEncode(writer);
            writer.Write(args.ChildId);
            writer.Write(args.parentId);
            writer.Write(args.worldPositionStays);
        }

        public void Decode(BinaryReader reader, INetworkObjectManager manager)
        {
            args = new SetParentArgs
            {
                ChildId = reader.ReadInt32(),
                parentId = reader.ReadInt32(),
                worldPositionStays = reader.ReadBoolean()
            };
        }
    }
}

