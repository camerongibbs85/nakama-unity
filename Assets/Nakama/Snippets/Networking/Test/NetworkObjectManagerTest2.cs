using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class NetworkObjectManagerTest2 : MonoBehaviour {
    private const int INNER = 10;
    private const int OUTER = 15;
    private const int STAGEDELAY = 1;
    private const int ACTIONDELAY = 200;
    private const int RADIUS = 3;
    public Transform Actor;
    public Transform Reflector;

    private void Reflect(NetworkObjectManager actor, NetworkObjectManager reflector)
    {
        reflector.handler.HandleNetworkPayload(actor.handler.GetNetworkPayload());
    }

    private async void Start()
    {
        NetworkObjectManager actor = new NetworkObjectManager(Actor);
        NetworkObjectManager reflector = new NetworkObjectManager(Reflector);

        for (int i = 0; i < OUTER; i++)
        {
            await Record(actor, reflector);
            await Task.Delay(System.TimeSpan.FromSeconds(STAGEDELAY));        
        }
    }

    private async Task Record(NetworkObjectManager actor, NetworkObjectManager reflector)
    {
        var proxy = actor.CreatePrimitive(PrimitiveType.Cube);
        for (int i = 0; i < INNER; i++)
        {
            proxy.SetLocalPosition(Random.onUnitSphere * RADIUS);
            Reflect(actor, reflector);
            await Task.Delay(System.TimeSpan.FromMilliseconds(ACTIONDELAY));
        }
        proxy.Destroy();
        Reflect(actor, reflector);
    }
}