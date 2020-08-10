using System.IO;
using UnityEngine;
using NetworkActionCreateFunction = System.Func<int, NetworkObject, System.IO.BinaryReader, INetworkAction>;

public partial class NetworkObjectManager
{
    public class Handler
    {
        private readonly NetworkObjectManager manager;
        private readonly MemoryStream networkStream;
        private readonly BinaryWriter streamWriter;
        public Handler(NetworkObjectManager manager)
        {
            this.manager = manager;
            networkStream = new MemoryStream();
            streamWriter = new BinaryWriter(networkStream);
        }

        public byte[] GetNetworkPayload()
        {
            foreach (var item in manager.objectsActions)
            {
                if(item.Value.Count > 0)
                {
                    streamWriter.Write(item.Key);            
                    while(item.Value.Count > 0)
                    {
                        INetworkAction action = item.Value.Dequeue();
                        action.Encode(streamWriter);
                    }
                    streamWriter.Write((byte)NetworkActionId.NetworkActionEnd);
                }
            }

            if(networkStream.Length > 0)
            {
                byte[] payload = networkStream.ToArray();
                networkStream.SetLength(0);
                return payload;
            }

            return null;
        }

        public void HandleNetworkPayload(byte[] payload)
        {
            using(MemoryStream stream = new MemoryStream(payload))
            {
                using(BinaryReader reader = new BinaryReader(stream))
                {
                    while(stream.Position < stream.Length)
                    {
                        HandleObject(reader);
                    }
                }
            }
        }
    
        private void HandleObject(BinaryReader reader)
        {
            int instanceId = reader.ReadInt32();
            while(HandleObjectNetworkAction(instanceId, reader)) {}
        }

        private bool HandleObjectNetworkAction(int instanceId, BinaryReader reader)
        {
            NetworkActionId networkActionId = (NetworkActionId)reader.ReadByte();
            NetworkActionCreateFunction networkActionCreateFunction;
            if(manager.actionCreateFunctions.TryGetValue(networkActionId, out networkActionCreateFunction))
            {
                NetworkObject networkObject;
                manager.objects.TryGetValue(instanceId, out networkObject);
                var function = networkActionCreateFunction(instanceId, networkObject, reader);
                function.Dispatch(manager);
                return true;
            }

            Debug.AssertFormat(networkActionId == NetworkActionId.NetworkActionEnd, "Un-Registered Network Action Received[{0}], Implement a handler", networkActionId.ToString());
            return false;
        }
    }
}