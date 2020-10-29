using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Networking.Manager;
using Networking.ObjectCreator;

namespace Networking.Tests
{
    public class NetworkObjectManagerTest3 : NetworkObjectManagerTest
    {
        private const int INNER = 10;
        private const int OUTER = 10;
        private const int STAGEDELAY = 1;
        private const int ACTIONDELAY = 33;
        private const int RADIUS = 3;
        public TMPro.TMP_Text ModeLabel;

        private async void Start()
        {
            base.Setup(transform, transform);

            Queue<byte[]> payloads = new Queue<byte[]>();

            for (int i = 0; i < OUTER; i++)
            {
                await Record(payloads, Recorder, recorderHandler);
                await Task.Delay(System.TimeSpan.FromSeconds(STAGEDELAY));
                await Replay(payloads, replayerHandler);
                await Task.Delay(System.TimeSpan.FromSeconds(STAGEDELAY));
            }

            ModeLabel.text = "Done";
        }

        private void Enqueue(Queue<byte[]> payloads, INetworkedManager recorder)
        {
            payloads.Enqueue(recorder.GetNetworkPayload());
        }

        private void Dequeue(Queue<byte[]> payloads, INetworkedManager recorder)
        {
            recorder.HandleNetworkPayload(payloads.Dequeue());
        }

        private async Task Replay(Queue<byte[]> payloads, INetworkedManager replayer)
        {
            ModeLabel.text = "Replay";
            for (int i = 0; i < INNER; i++)
            {
                Dequeue(payloads, replayer);
                await Task.Delay(System.TimeSpan.FromMilliseconds(ACTIONDELAY));
            }
            Dequeue(payloads, replayer);
        }

        private async Task Record(Queue<byte[]> payloads, INetworkObjectManager recorder, INetworkedManager recorderHandler)
        {
            ModeLabel.text = "Record";
            var proxy = recorder.Create<PrimitiveCreator>((creator) => creator.Create(PrimitiveType.Cube)); //CreatePrimitive(PrimitiveType.Cube);
            for (int i = 0; i < INNER; i++)
            {
                proxy.SetLocalPosition(Random.onUnitSphere * RADIUS);
                Enqueue(payloads, recorderHandler);
                await Task.Delay(System.TimeSpan.FromMilliseconds(ACTIONDELAY));
            }
            proxy.Destroy();
            Enqueue(payloads, recorderHandler);
        }
    }
}

