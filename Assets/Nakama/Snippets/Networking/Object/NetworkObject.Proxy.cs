using UnityEngine;

public partial class NetworkObject
{
    public class Proxy
    {
        public const int NullInstanceId = int.MinValue;
        public static (int instanceId, NetworkObject networkObject, Proxy proxy) Create(GameObject gameObject, int instanceId = NullInstanceId)
        {
            var newId = instanceId == int.MinValue ? gameObject.GetInstanceID() : instanceId;
            var networkObject = new NetworkObject(newId, gameObject);
            var proxy = new Proxy(networkObject);
            return (newId, networkObject, proxy);
        }
        private readonly NetworkObject networkObject;
        private Proxy(NetworkObject networkObject)
        {
            this.networkObject = networkObject;
        }

        public INetworkAction SetLocalPosition(Vector3 position)
        {
            return NetworkObjectManager.PositionAction.Create(networkObject.instanceId, networkObject, position);
        }

        public INetworkAction Destroy()
        {
            return NetworkObjectManager.DestroyAction.Create(networkObject.instanceId, networkObject);
        }
    }
}
