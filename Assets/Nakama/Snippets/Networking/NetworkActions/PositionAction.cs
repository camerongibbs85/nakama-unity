using System.IO;
using UnityEngine;

public partial class NetworkObjectManager
{
    public class PositionAction : INetworkAction
    {
        private static Vector3 tempPosition;
        public static PositionAction Create(int instanceId, NetworkObject networkObject, BinaryReader reader)
        {
            tempPosition.x = reader.ReadSingle();
            tempPosition.y = reader.ReadSingle();
            tempPosition.z = reader.ReadSingle();
            return new PositionAction(instanceId, networkObject, tempPosition);
        }
        public void Encode(BinaryWriter writer)
        {
            writer.Write((byte)NetworkActionId.Postition);
            writer.Write(position.x);
            writer.Write(position.y);
            writer.Write(position.z);
        }
        public static PositionAction Create(int instanceId, NetworkObject networkObject, Vector3 position)
        {
            return new PositionAction(instanceId, networkObject, position);
        }

        private readonly int instanceId;
        private readonly NetworkObject networkObject;
        private readonly Vector3 position;
        private PositionAction(int instanceId, NetworkObject networkObject, Vector3 position)
        {  
            this.instanceId = instanceId;
            this.networkObject = networkObject;
            this.position = position;
        }

        public int Dispatch(NetworkObjectManager networkObjectManager)
        {
            networkObject.SetLocalPosition(position);
            return instanceId;
        }
    }
}