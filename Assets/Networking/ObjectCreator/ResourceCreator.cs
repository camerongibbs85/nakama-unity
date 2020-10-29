using Networking.Manager;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Networking.ObjectCreator
{
    [NetworkCreatorId(NetworkCreatorId.Resource)]
    [CreateAssetMenu(fileName = "ResourceCreator", menuName = "nakama-unity/ResourceCreator", order = 0)]
    public class ResourceCreator : ScriptableObject, INetworkedCreator
    {
        [SerializeField]
        private List<Resource> resourceList = new List<Resource>();

        public CreatorResult Create(string resourceName)
        {
            var index = resourceList.FindIndex(res => res.Name == resourceName);

            if (index < 0)
            {
                throw new System.ArgumentException($"{resourceName} doesn't exist in Resource List.", "resourceName");
            }

            byte[] creatorBytes = new[] { (byte)index };
            return new CreatorResult(this.GetId(), creatorBytes);
        }

        public byte[] ExtractBytes(BinaryReader reader)
        {
            return new[] { reader.ReadByte() };
        }

        public GameObject UseBytes(byte[] bytes, Transform parentTransform)
        {
            var index = (int)bytes[0];
            return CreateGameObject(resourceList[index], parentTransform);
        }

        private static GameObject CreateGameObject(Resource resource, Transform parentTransform)
        {
            var gameObject = resource.GetInstance();
            gameObject.transform.SetParent(parentTransform);
            return gameObject;
        }
    }
}

