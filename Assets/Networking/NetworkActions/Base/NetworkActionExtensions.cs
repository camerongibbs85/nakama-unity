using System.IO;
using Networking.Support;

namespace Networking.NetworkActions
{
    public static class NetworkActionExtensions
    {
        private static TypeIdMap<NetworkActionIdAttribute, NetworkActionId> typeIdMap = new TypeIdMap<NetworkActionIdAttribute, NetworkActionId>();

        public static void BaseEncode<T>(this T networkAction, BinaryWriter writer) where T : class, INetworkAction
        {
            writer.Write((byte)networkAction.GetId());
        }

        public static NetworkActionId GetId<T>(this T networkAction) where T : INetworkAction
        {
            return typeIdMap.GetTypeId(typeof(T));
        }

        public static INetworkAction Create<T>(this T networkAction) where T : INetworkAction, new()
        {
            return new T();
        }
    }
}
