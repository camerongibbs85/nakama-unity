using System;
using Networking.Support;

namespace Networking.ObjectCreator
{
    public class NetworkCreatorIdAttribute : Attribute, IAttributeId<NetworkCreatorId>
    {
        public NetworkCreatorId networkCreatorId;
        public NetworkCreatorId Id => networkCreatorId;
        public NetworkCreatorIdAttribute(NetworkCreatorId networkCreatorId)
        {
            this.networkCreatorId = networkCreatorId;
        }
    }
}

