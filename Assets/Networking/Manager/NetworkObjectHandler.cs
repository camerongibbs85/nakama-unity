using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Networking.NetworkObjects;
using Networking.NetworkActions;


namespace Networking.Manager
{
    public class NetworkObjectHandler : INetworkObjectHandler
    {
        private readonly List<Tuple<int, Queue<INetworkAction>>> networkObjectActionQueues;

        private readonly INetworkObjectManager manager;
        private readonly MemoryStream networkStream;
        private readonly BinaryWriter streamWriter;

        public NetworkObjectHandler(INetworkObjectManager manager)
        {
            this.manager = manager;
            this.manager.SetHandler(this);
            networkStream = new MemoryStream();
            streamWriter = new BinaryWriter(networkStream);
            networkObjectActionQueues = new List<Tuple<int, Queue<INetworkAction>>>();
        }

        public int NetworkObjectFunction(int instanceId, INetworkAction objectAction)
        {
            var newId = objectAction.Dispatch(instanceId, manager);
            EnqueueObjectAction(newId, objectAction);
            return newId;
        }

        public void EnqueueObjectAction(int instanceId, INetworkAction objectAction)
        {
            Queue<INetworkAction> objectActionQueue;
            int index = networkObjectActionQueues.FindLastIndex(item => item.Item1 == instanceId);
            if (index > -1 && index == networkObjectActionQueues.Count - 1)
            {
                objectActionQueue = networkObjectActionQueues[index].Item2;
            }
            else
            {
                objectActionQueue = new Queue<INetworkAction>();
                networkObjectActionQueues.Add(new Tuple<int, Queue<INetworkAction>>(instanceId, objectActionQueue));
            }
            objectActionQueue.Enqueue(objectAction);
        }

        public byte[] GetNetworkPayload()
        {
            foreach (var item in networkObjectActionQueues)
            {
                if (item.Item2.Count > 0)
                {
                    streamWriter.Write(item.Item1);
                    while (item.Item2.Count > 0)
                    {
                        INetworkAction action = item.Item2.Dequeue();
                        action.Encode(streamWriter);
                    }
                    streamWriter.Write((byte)NetworkActionId.NetworkActionEnd);
                }
            }
            networkObjectActionQueues.Clear();

            if (networkStream.Length > 0)
            {
                byte[] payload = networkStream.ToArray();
                networkStream.SetLength(0);
                return payload;
            }

            return null;
        }

        public void HandleNetworkPayload(byte[] payload)
        {
            using (MemoryStream stream = new MemoryStream(payload))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    while (stream.Position < stream.Length)
                    {
                        HandleObject(reader);
                    }
                }
            }
        }

        private void HandleObject(BinaryReader reader)
        {
            int instanceId = reader.ReadInt32();
            while (HandleObjectNetworkAction(instanceId, reader)) { }
        }

        private bool HandleObjectNetworkAction(int instanceId, BinaryReader reader)
        {
            NetworkActionId networkActionId = (NetworkActionId)reader.ReadByte();
            if (NetworkActionRegistry.TryGetValue(networkActionId, out NetworkActionCreateFunction networkActionCreateFunction))
            {
                manager.TryGetObject(instanceId, out NetworkObject networkObject);
                var action = networkActionCreateFunction();
                action.Decode(reader, manager);
                action.Dispatch(instanceId, manager);
                return true;
            }

            Debug.AssertFormat(networkActionId == NetworkActionId.NetworkActionEnd, "Un-Registered Network Action Received[{0}], Implement a handler", networkActionId.ToString());
            return false;
        }
    }
}

