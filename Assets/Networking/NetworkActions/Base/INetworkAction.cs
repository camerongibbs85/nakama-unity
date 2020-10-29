using System.IO;
using Networking.Manager;

namespace Networking.NetworkActions
{
    public interface INetworkAction
    {
        int Dispatch(int instanceId, INetworkObjectManager networkObjectManager);
        void Encode(BinaryWriter writer);
        void Decode(BinaryReader reader, INetworkObjectManager manager);
    }
}

