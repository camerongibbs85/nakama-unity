using Networking.Manager;
using System.IO;
using UnityEngine;

namespace Networking.ObjectCreator
{
    [NetworkCreatorId(NetworkCreatorId.Primitive)]
    public class PrimitiveCreator : ScriptableObject, INetworkedCreator
    {
        public CreatorResult Create(PrimitiveType primitiveType)
        {
            byte[] creatorBytes = new[] { (byte)primitiveType };
            return new CreatorResult(this.GetId(), creatorBytes);
        }

        public byte[] ExtractBytes(BinaryReader reader)
        {
            return new[] { reader.ReadByte() };
        }

        public GameObject UseBytes(byte[] bytes, Transform parentTransform)
        {
            var primitiveType = (PrimitiveType)bytes[0];
            // create a gameobject
            return CreateGameObject(primitiveType, parentTransform);
        }

        private static GameObject CreateGameObject(PrimitiveType primitiveType, Transform parentTransform)
        {
            var gameObject = GameObject.CreatePrimitive(primitiveType);
            gameObject.transform.SetParent(parentTransform);
            return gameObject;
        }
    }
}

