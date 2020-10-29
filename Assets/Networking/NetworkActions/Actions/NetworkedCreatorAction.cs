using System.IO;
using Networking.Manager;
using Networking.ObjectCreator;

namespace Networking.NetworkActions
{
    [NetworkActionId(NetworkActionId.NetworkedCreator)]
    public class NetworkedCreatorAction : INetworkAction
    {
        private NetworkCreatorId creatorId;
        private byte[] bytes;
        public NetworkedCreatorAction() { }
        public NetworkedCreatorAction(NetworkCreatorId creatorId, byte[] bytes)
        {
            this.creatorId = creatorId;
            this.bytes = bytes;
        }

        public int Dispatch(int instanceId, INetworkObjectManager networkObjectManager)
        {
            return networkObjectManager.UseCreator(creatorId, bytes, instanceId);
        }

        public void Encode(BinaryWriter writer)
        {
            this.BaseEncode(writer);
            writer.Write((byte)creatorId);
            writer.Write(bytes);
        }

        public void Decode(BinaryReader reader, INetworkObjectManager manager)
        {
            creatorId = (NetworkCreatorId)reader.ReadByte();
            bytes = manager.GetCreator(creatorId).ExtractBytes(reader);
        }
    }
}

