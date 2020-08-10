using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class NetworkObjectManagerTest : MonoBehaviour {
    private const int INNER = 10;
    private const int OUTER = 15;
    private const int STAGEDELAY = 1;
    private const int ACTIONDELAY = 200;

    private void Enqueue(Queue<byte[]> payloads, NetworkObjectManager manager)
    {
        payloads.Enqueue(manager.handler.GetNetworkPayload());
    }

    private void Dequeue(Queue<byte[]> payloads, NetworkObjectManager manager)
    {
        manager.handler.HandleNetworkPayload(payloads.Dequeue());
    }

    private async void Start()
    {
        Queue<byte[]> payloads = new Queue<byte[]>();
        NetworkObjectManager manager = new NetworkObjectManager(transform);
        NetworkObjectManager manager1 = new NetworkObjectManager(transform);

        for (int i = 0; i < OUTER; i++)
        {
            await Record(payloads, manager);
            await Task.Delay(System.TimeSpan.FromSeconds(STAGEDELAY));
            await Replay(payloads, manager1);
            await Task.Delay(System.TimeSpan.FromSeconds(STAGEDELAY));            
        }
    }

    private async Task Replay(Queue<byte[]> payloads, NetworkObjectManager manager1)
    {
        for (int i = 0; i < INNER; i++)
        {
            Dequeue(payloads, manager1);
            await Task.Delay(System.TimeSpan.FromMilliseconds(ACTIONDELAY));
        }
        Dequeue(payloads, manager1);
    }

    private async Task Record(Queue<byte[]> payloads, NetworkObjectManager manager)
    {
        var proxy = manager.CreatePrimitive(PrimitiveType.Cube);
        for (int i = 0; i < INNER; i++)
        {
            manager.NetworkObjectFunction(() => proxy.SetLocalPosition(Random.onUnitSphere * STAGEDELAY));
            Enqueue(payloads, manager);
            await Task.Delay(System.TimeSpan.FromMilliseconds(ACTIONDELAY));
        }
        manager.NetworkObjectFunction(() => proxy.Destroy());
        Enqueue(payloads, manager);
    }
}