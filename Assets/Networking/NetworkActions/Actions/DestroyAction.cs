using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Networking.Manager;
using Networking.NetworkObjects;

namespace Networking.NetworkActions
{
    [NetworkActionId(NetworkActionId.Destroy)]
    public class DestroyAction : INetworkAction
    {
        private DestroyArgs args;
        public DestroyAction() { }
        public DestroyAction(DestroyArgs args)
        {
            this.args = args;
        }

        public int Dispatch(int instanceId, INetworkObjectManager networkObjectManager)
        {
            networkObjectManager.TryGetObject(instanceId, out NetworkObject networkObject);
            if (networkObject != null)
            {
                networkObject.Destroy(args, networkObjectManager);
            }
            return instanceId;
        }

        public void Encode(BinaryWriter writer)
        {
            this.BaseEncode(writer);
            writer.Write(args.ChildId);
        }

        public void Decode(BinaryReader reader, INetworkObjectManager manager)
        {
            args = new DestroyArgs
            {
                ChildId = reader.ReadInt32()
            };
        }
    }
}

