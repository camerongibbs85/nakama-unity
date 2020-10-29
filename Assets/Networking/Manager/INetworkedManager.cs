using System;
using UnityEngine;

namespace Networking.Manager
{
    public interface INetworkedManager
    {
        byte[] GetNetworkPayload();
        void HandleNetworkPayload(byte[] payload);
    }
}

