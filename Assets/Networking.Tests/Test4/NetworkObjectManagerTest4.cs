using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Networking.Manager;
using Networking.ObjectCreator;

namespace Networking.Tests
{
    public class NetworkObjectManagerTest4 : NetworkObjectManagerTest
    {
        private const int INNER = 10;
        private const int OUTER = 15;
        private const int STAGEDELAY = 1;
        private const int ACTIONDELAY = 200;
        private const int RADIUS = 3;
        public Transform ActorTransform;
        public Transform ReflectorTransform;

        private async void Start()
        {
            base.Setup(ActorTransform, ReflectorTransform);

            for (int i = 0; i < OUTER; i++)
            {
                await Record(Recorder, recorderHandler, replayerHandler);
                await Task.Delay(System.TimeSpan.FromSeconds(STAGEDELAY));
            }
        }

        private async Task Record(INetworkObjectManager actor, INetworkedManager actorHandler, INetworkedManager reflectorHandler)
        {
            var proxy = actor.Create<PrimitiveCreator>((creator) => creator.Create(PrimitiveType.Cube)); //CreatePrimitive(PrimitiveType.Cube);
            for (int i = 0; i < INNER; i++)
            {
                proxy.SetLocalPosition(Random.onUnitSphere * RADIUS);
                Reflect(actorHandler, reflectorHandler);
                await Task.Delay(System.TimeSpan.FromMilliseconds(ACTIONDELAY));
            }
            proxy.Destroy();
            Reflect(actorHandler, reflectorHandler);
        }

        private void Reflect(INetworkedManager actor, INetworkedManager reflector)
        {
            reflector.HandleNetworkPayload(actor.GetNetworkPayload());
        }
    }
}

