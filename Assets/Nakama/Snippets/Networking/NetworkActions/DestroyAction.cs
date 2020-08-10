using System.IO;
using UnityEngine;

public partial class NetworkObjectManager
{
    public class DestroyAction : INetworkAction
    {
        public static DestroyAction Create(int instanceId, NetworkObject networkObject, BinaryReader reader)
        {
            return Create(instanceId, networkObject);
        }
        public void Encode(BinaryWriter writer)
        {
            writer.Write((byte)NetworkActionId.Destroy);
        }
        public static DestroyAction Create(int instanceId, NetworkObject networkObject)
        {
            return new DestroyAction(instanceId, networkObject);
        }

        private readonly int instanceId;
        private readonly NetworkObject networkObject;
        private DestroyAction(int instanceId, NetworkObject networkObject)
        {  
            this.instanceId = instanceId;
            this.networkObject = networkObject;
        }

        public int Dispatch(NetworkObjectManager networkObjectManager)
        {
            networkObjectManager.DestroyNetworkObject(networkObject);
            return instanceId;
        }
    }
}