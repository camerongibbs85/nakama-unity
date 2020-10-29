using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Networking.Manager;
using Networking.ObjectCreator;
using System.Collections;
using Networking.NetworkObjects;
using static Networking.NetworkObjects.NetworkObject;

namespace Networking.Tests
{
    public class NetworkObjectManagerTest7 : NetworkObjectManagerTest
    {
        private const int INNER = 10;
        private const int OUTER = 10;
        private const int STAGEDELAY = 1;
        private const int ACTIONDELAY = 132;
        private const int RADIUS = 3;
        public TMPro.TMP_Text ModeLabel;

        WaitForSeconds stageDelay;
        WaitForSeconds actionDelay;

        [SerializeField]
        private List<ProxyMono> proxyMonoList = new List<ProxyMono>();
        private List<Proxy> proxies;


        private void Awake()
        {
            stageDelay = new WaitForSeconds((float)System.TimeSpan.FromSeconds(STAGEDELAY).TotalSeconds);
            actionDelay = new WaitForSeconds((float)System.TimeSpan.FromMilliseconds(ACTIONDELAY).TotalSeconds);
        }

        private IEnumerator Start()
        {
            base.Setup(transform, transform);
            proxies = proxyMonoList.ConvertAll(pm => pm.AddToManager(Recorder));
            proxyMonoList.ConvertAll(pm => pm.AddToManager(Replayer));

            Queue<byte[]> payloads = new Queue<byte[]>();

            for (int i = 0; i < OUTER; i++)
            {
                var record = Record(payloads, Recorder, recorderHandler);
                while(record.MoveNext())
                {
                    yield return record.Current;
                }
                yield return stageDelay;
                var replay = Replay(payloads, replayerHandler);
                while(replay.MoveNext())
                {
                    yield return replay.Current;
                }
                yield return stageDelay;
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

        private IEnumerator Replay(Queue<byte[]> payloads, INetworkedManager replayer)
        {
            ModeLabel.text = "Replay";
            for (int i = 0; i < INNER; i++)
            {
                Dequeue(payloads, replayer);
                yield return actionDelay;
            }
            Dequeue(payloads, replayer);
        }

        private IEnumerator Record(Queue<byte[]> payloads, INetworkObjectManager recorder, INetworkedManager recorderHandler)
        {
            ModeLabel.text = "Record";
            var proxy = recorder.Create<PrimitiveCreator>((creator) => creator.Create(PrimitiveType.Cube)); //CreatePrimitive(PrimitiveType.Cube);
            for (int i = 0; i < INNER; i++)
            {
                proxy.SetLocalPosition(Random.onUnitSphere * RADIUS);
                proxies.ForEach(p => p.SetLocalPosition(Random.onUnitSphere * RADIUS));
                Enqueue(payloads, recorderHandler);
                yield return actionDelay;
            }
            proxy.Destroy();
            Enqueue(payloads, recorderHandler);
        }
    }
}

