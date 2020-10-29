using Networking.NetworkActions;

namespace Networking.Manager
{
    public interface INetworkObjectHandler : INetworkedManager
    {
        int NetworkObjectFunction(int instanceId, INetworkAction objectAction);
        void EnqueueObjectAction(int instanceId, INetworkAction objectAction);
    }
}

