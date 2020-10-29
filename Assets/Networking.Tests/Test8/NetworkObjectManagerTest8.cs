using System.Threading.Tasks;
using UnityEngine;
using Networking.Manager;
using Networking.ObjectCreator;
using System.Collections;

namespace Networking.Tests
{
    public class NetworkObjectManagerTest8 : NetworkObjectManagerTest
    {
        private const int INNER = 10;
        private const int OUTER = 15;
        private const int STAGEDELAY = 1;
        private const int ACTIONDELAY = 200;
        private const int RADIUS = 3;
        public Transform ActorTransform;
        public Transform ReflectorTransform;

        WaitForSeconds stageDelay;
        WaitForSeconds actionDelay;

        private void Awake()
        {
            stageDelay = new WaitForSeconds((float)System.TimeSpan.FromSeconds(STAGEDELAY).TotalSeconds);
            actionDelay = new WaitForSeconds((float)System.TimeSpan.FromMilliseconds(ACTIONDELAY).TotalSeconds);
        }

        private IEnumerator Start()
        {
            base.Setup(ActorTransform, ReflectorTransform);

            for (int i = 0; i < OUTER; i++)
            {
                var record = Record(Recorder, recorderHandler, replayerHandler);
                while (record.MoveNext())
                {
                    yield return record.Current;
                }
                yield return stageDelay;
            }
        }

        private IEnumerator Record(INetworkObjectManager actor, INetworkedManager actorHandler, INetworkedManager reflectorHandler)
        {
            var proxy = actor.Create<ResourceCreator>((creator) => creator.Create("Test"));
            for (int i = 0; i < INNER; i++)
            {
                proxy.SetLocalPosition(Random.onUnitSphere * RADIUS);
                Reflect(actorHandler, reflectorHandler);
                yield return actionDelay;
            }
            // ensure that the next bunch of actions get sent in the correct order
            // this tests that the parent gets created before the proxy gets childed and moved
            // even if the proxy has a move before the object creation
            proxy.SetLocalPosition(Random.onUnitSphere * RADIUS);
            var parentId = proxy.RegisterChild("child/sphere");
            var child = actor.Create<PrimitiveCreator>((creator) => creator.Create(PrimitiveType.Sphere));
            child.SetParent(proxy, true, parentId);
            child.SetLocalPosition(new Vector3(1,1,1).normalized);
            Reflect(actorHandler, reflectorHandler);
            yield return stageDelay;

            for (int i = 0; i < INNER; i++)
            {
                proxy.SetLocalPosition(Random.onUnitSphere * RADIUS);
                child.SetLocalPosition(Random.onUnitSphere * RADIUS);
                Reflect(actorHandler, reflectorHandler);
                yield return actionDelay;
            }
            child.Destroy();
            proxy.Destroy();
            Reflect(actorHandler, reflectorHandler);
        }

        private void Reflect(INetworkedManager actor, INetworkedManager reflector)
        {
            reflector.HandleNetworkPayload(actor.GetNetworkPayload());
        }
    }
}