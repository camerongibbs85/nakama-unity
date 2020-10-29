using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Networking.Manager;
using Networking.ObjectCreator;

namespace Networking.Tests
{
    public class NetworkObjectManagerTest5 : NetworkObjectManagerTest
    {
        private const int INNER = 10;
        private const int OUTER = 15;
        private const int STAGEDELAY = 1;
        private const int ACTIONDELAY = 200;
        private const int RADIUS = 3;
        public Transform ActorTransform;
        public Transform ReflectorTransform;

        private void Reflect(INetworkedManager actor, INetworkedManager reflector)
        {
            reflector.HandleNetworkPayload(actor.GetNetworkPayload());
        }

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
            // ensure that the next bunch of actions get sent in the correct order
            // this tests that the parent gets created before the proxy gets childed and moved
            // even if the proxy has a move before the object creation
            proxy.SetLocalPosition(Random.onUnitSphere * RADIUS);
            var parent = actor.Create<PrimitiveCreator>((creator) => creator.Create(PrimitiveType.Sphere)); //CreatePrimitive(PrimitiveType.Sphere);
            parent.SetLocalPosition(new Vector3(1, 3));
            proxy.SetParent(parent, true);
            Reflect(actorHandler, reflectorHandler);
            await Task.Delay(System.TimeSpan.FromMilliseconds(ACTIONDELAY));

            for (int i = 0; i < INNER; i++)
            {
                proxy.SetLocalPosition(Random.onUnitSphere * RADIUS);
                Reflect(actorHandler, reflectorHandler);
                await Task.Delay(System.TimeSpan.FromMilliseconds(ACTIONDELAY));
            }
            proxy.Destroy();
            parent.Destroy();
            Reflect(actorHandler, reflectorHandler);
        }
    }
}

