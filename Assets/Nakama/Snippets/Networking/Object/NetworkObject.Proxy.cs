using UnityEngine;

public partial class NetworkObject
{
    public class Proxy
    {
        public const int NullInstanceId = int.MinValue;
        public static (int instanceId, NetworkObject networkObject, Proxy proxy) Create(NetworkObjectManager networkObjectManager, GameObject gameObject, int instanceId = NullInstanceId)
        {
            var newId = instanceId == int.MinValue ? gameObject.GetInstanceID() : instanceId;
            var networkObject = new NetworkObject(newId, gameObject, networkObjectManager);
            var proxy = new Proxy(networkObject);
            return (newId, networkObject, proxy);
        }
        private readonly NetworkObject networkObject;
        private Proxy(NetworkObject networkObject)
        {
            this.networkObject = networkObject;
        }

        public void SetLocalPosition(Vector3 position)
        {
            networkObject.networkObjectManager.NetworkObjectFunction(NetworkObjectManager.PositionAction.Create(networkObject.instanceId, networkObject, position));
        }

        public void Destroy()
        {
            networkObject.networkObjectManager.NetworkObjectFunction(NetworkObjectManager.DestroyAction.Create(networkObject.instanceId, networkObject));
        }
    }
}
