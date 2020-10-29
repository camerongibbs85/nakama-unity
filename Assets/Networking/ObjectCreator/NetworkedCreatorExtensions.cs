using Networking.Support;
using System.IO;

namespace Networking.ObjectCreator
{
    public static class NetworkedCreatorExtensions
    {
        private static TypeIdMap<NetworkCreatorIdAttribute, NetworkCreatorId> typeIdMap = new TypeIdMap<NetworkCreatorIdAttribute, NetworkCreatorId>();

        public static NetworkCreatorId GetId(this INetworkedCreator networkedCreator)
        {
            return typeIdMap.GetTypeId(networkedCreator.GetType());
        }

        public static NetworkCreatorId GetId<T>(this T networkedCreator) where T : INetworkedCreator
        {
            return typeIdMap.GetTypeId(typeof(T));
        }
    }
}
