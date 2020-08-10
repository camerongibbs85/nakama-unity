using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


public class NetworkObjectManagerTest : MonoBehaviour {
    private const int INNER = 10;
    private const int OUTER = 10;
    private const int STAGEDELAY = 1;
    private const int ACTIONDELAY = 33;
    private const int RADIUS = 3;
    public TMPro.TMP_Text ModeLabel;

    private void Enqueue(Queue<byte[]> payloads, NetworkObjectManager recorder)
    {
        payloads.Enqueue(recorder.handler.GetNetworkPayload());
    }

    private void Dequeue(Queue<byte[]> payloads, NetworkObjectManager recorder)
    {
        recorder.handler.HandleNetworkPayload(payloads.Dequeue());
    }

    private async void Start()
    {
        Queue<byte[]> payloads = new Queue<byte[]>();
        NetworkObjectManager recorder = new NetworkObjectManager(transform);
        NetworkObjectManager replayer = new NetworkObjectManager(transform);

        for (int i = 0; i < OUTER; i++)
        {
            await Record(payloads, recorder);
            await Task.Delay(System.TimeSpan.FromSeconds(STAGEDELAY));
            await Replay(payloads, replayer);
            await Task.Delay(System.TimeSpan.FromSeconds(STAGEDELAY));            
        }

        ModeLabel.text = "Done";
    }

    private async Task Replay(Queue<byte[]> payloads, NetworkObjectManager replayer)
    {        
        ModeLabel.text = "Replay";
        for (int i = 0; i < INNER; i++)
        {
            Dequeue(payloads, replayer);
            await Task.Delay(System.TimeSpan.FromMilliseconds(ACTIONDELAY));
        }
        Dequeue(payloads, replayer);
    }

    private async Task Record(Queue<byte[]> payloads, NetworkObjectManager recorder)
    {
        ModeLabel.text = "Record";
        var proxy = recorder.CreatePrimitive(PrimitiveType.Cube);
        for (int i = 0; i < INNER; i++)
        {
            proxy.SetLocalPosition(Random.onUnitSphere * RADIUS);
            Enqueue(payloads, recorder);
            await Task.Delay(System.TimeSpan.FromMilliseconds(ACTIONDELAY));
        }
        proxy.Destroy();
        Enqueue(payloads, recorder);
    }
}