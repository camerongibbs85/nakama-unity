using System.IO;
using UnityEngine;

public partial class NetworkObjectManager
{
    public class CreatePrimitiveAction : INetworkAction
    {
        public static CreatePrimitiveAction Create(int instanceId, NetworkObject networkObject, BinaryReader reader)
        {
            PrimitiveType primitiveType = (PrimitiveType)reader.ReadByte();
            return Create(instanceId, networkObject, primitiveType);
        }
        public void Encode(BinaryWriter writer)
        {
            writer.Write((byte)NetworkActionId.CreatePrimitive);
            writer.Write((byte)primitiveType);
        }
        public static CreatePrimitiveAction Create(int instanceId, NetworkObject networkObject, PrimitiveType primitiveType)
        {
            return new CreatePrimitiveAction(instanceId, networkObject, primitiveType);
        }

        private readonly int instanceId;
        NetworkObject networkObject;
        private readonly PrimitiveType primitiveType;
        private CreatePrimitiveAction(int instanceId, NetworkObject networkObject, PrimitiveType primitiveType)
        {  
            this.instanceId = instanceId;
            this.networkObject = networkObject;
            this.primitiveType = primitiveType;
        }

        public int Dispatch(NetworkObjectManager networkObjectManager)
        {
            networkObjectManager.CreatePrimitive(primitiveType, instanceId);
            return instanceId;
        }
    }
}