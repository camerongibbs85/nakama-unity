using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using NetworkActionCreateFunction = System.Func<int, NetworkObject, System.IO.BinaryReader, INetworkAction>;

public partial class NetworkObjectManager
{
    private readonly Transform parentTransform;
    private readonly Dictionary<int, NetworkObject> objects;
    private readonly Dictionary<int, Queue<INetworkAction>> objectsActions;
    private readonly Dictionary<NetworkActionId, NetworkActionCreateFunction> actionCreateFunctions;
    private readonly Queue<INetworkAction> networkActionsToDispatch;
    public readonly Handler handler;

    public NetworkObjectManager(Transform parentTransform)
    {
        this.parentTransform = parentTransform;
        objects = new Dictionary<int, NetworkObject>();
        objectsActions = new Dictionary<int, Queue<INetworkAction>>();
        actionCreateFunctions = new Dictionary<NetworkActionId, NetworkActionCreateFunction>{
            { NetworkActionId.CreatePrimitive, CreatePrimitiveAction.Create },
            { NetworkActionId.Postition, PositionAction.Create },
            { NetworkActionId.Destroy, DestroyAction.Create }
        };
        networkActionsToDispatch = new Queue<INetworkAction>();
        handler = new Handler(this);
    }

    private void EnqueueObjectAction(int instanceId, INetworkAction objectAction)
    {
        Queue<INetworkAction> objectActionQueue;
        if(!objectsActions.TryGetValue(instanceId, out objectActionQueue))
        {
            objectActionQueue = new Queue<INetworkAction>();
            objectsActions.Add(instanceId, objectActionQueue);
        }
        objectActionQueue.Enqueue(objectAction);
    }

    private void DestroyNetworkObject(NetworkObject networkObject)
    {
        objects.Remove(networkObject.Destroy());
    }

    public NetworkObject.Proxy CreatePrimitive(PrimitiveType type, int instanceId = NetworkObject.NullInstanceId)
    {
        // create a gameobject
        var gameObject = GameObject.CreatePrimitive(type);
        gameObject.transform.SetParent(parentTransform);
    
        // wrap it with a network object
        var creation = NetworkObject.Proxy.Create(this, gameObject, instanceId);

        // keep track of it
        objects.Add(creation.instanceId, creation.networkObject);

        if(instanceId == NetworkObject.NullInstanceId)
        {
            // if this object was created locally, it won't have an instanceId until it is created
            // create a network action to record it's creation (that will get serialised into the stream with other object actions)
            INetworkAction objectAction = CreatePrimitiveAction.Create(creation.instanceId, creation.networkObject, type);
            EnqueueObjectAction(creation.instanceId, objectAction);
        }

        // return the proxy (so that users can't directly manipulate the object. Instead they use its proxy that will manipulate the object through network actions)
        return creation.proxy;
    }

    public void NetworkObjectFunction(INetworkAction objectAction)
    {
        var instanceId = objectAction.Dispatch(this);
        EnqueueObjectAction(instanceId, objectAction);
    }
}