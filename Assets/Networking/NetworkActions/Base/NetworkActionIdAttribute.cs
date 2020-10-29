using System;
using Networking.Support;

namespace Networking.NetworkActions
{
    public class NetworkActionIdAttribute : Attribute, IAttributeId<NetworkActionId>
    {
        public NetworkActionId networkActionId;
        public NetworkActionId Id => networkActionId;
        public NetworkActionIdAttribute(NetworkActionId networkActionId)
        {
            this.networkActionId = networkActionId;
        }
    }
}

