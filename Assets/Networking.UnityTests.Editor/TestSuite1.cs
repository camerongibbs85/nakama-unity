using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Networking.NetworkActions;
using Networking.Manager;
using Networking.ObjectCreator;

namespace Networking.UnityTests
{
    internal class TestCreator : INetworkedCreator
    {
        public NetworkCreatorId GetId()
        {
            throw new System.NotImplementedException();
        }

        public byte[] ExtractBytes(BinaryReader reader)
        {
            throw new System.NotImplementedException();
        }

        public GameObject UseBytes(byte[] bytes, Transform parentTransform)
        {
            throw new System.NotImplementedException();
        }
    }

    internal class TestHandler : INetworkObjectHandler
    {
        public void EnqueueObjectAction(int instanceId, INetworkAction objectAction)
        {
        }

        public byte[] GetNetworkPayload()
        {
            throw new System.NotImplementedException();
        }

        public void HandleNetworkPayload(byte[] payload)
        {
            throw new System.NotImplementedException();
        }

        public int NetworkObjectFunction(int instanceId, INetworkAction objectAction)
        {
            return int.MinValue;
        }
    }

    public class EditorTestSuite1
    {
        static Queue<Object> unityDestroyables = new Queue<Object>();

        [SetUp]
        public void SetUp()
        {
        }

        [TearDown]
        public void TearDown()
        {
            Object obj;
            while (unityDestroyables.Count > 0 && (obj = unityDestroyables.Dequeue()))
            {
                Object.DestroyImmediate(obj);
            }
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator PrimitiveCreator_Creates_Expected_PrimitiveType()
        {
            var gameObject = new GameObject();
            var parent = gameObject.transform;
            unityDestroyables.Enqueue(gameObject);
            yield return null;

            PrimitiveCreator primitiveCreator = ScriptableObject.CreateInstance<PrimitiveCreator>();
            var (id, bytes) = primitiveCreator.Create(PrimitiveType.Sphere);
            primitiveCreator.UseBytes(bytes, parent);
            yield return null;

            UnityEngine.Assertions.Assert.IsNotNull(parent.Find("Sphere"));
        }

        [Test]
        public void NetworkObjectManager_Uses_Expected_Creator()
        {
            var expectedCreator = new TestCreator();
            INetworkedCreator usedCreator = null;
            INetworkObjectManager test = ScriptableObject.CreateInstance<NetworkObjectManager>();
            test.SetHandler(new TestHandler());
            test.NetworkedCreatorList = new List<INetworkedCreator> { expectedCreator };
            test.Create<TestCreator>((c) =>
            {
                usedCreator = c;
                return new CreatorResult(NetworkCreatorId.Primitive, new byte[0]);
            });

            UnityEngine.Assertions.Assert.AreEqual(usedCreator, expectedCreator);
        }
    }
}

