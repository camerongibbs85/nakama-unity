using UnityEngine;

namespace Networking.ObjectCreator
{
    [System.Serializable]
    public class Resource
    {
        [SerializeField]
        private string name;
        [SerializeField]
        private string path;
        private GameObject gameObject;

        public string Name { get { return name; } }
        public string Path { get { return path; } }

        public GameObject GetInstance()
        {
            if (gameObject == null)
            {
                gameObject = Resources.Load<GameObject>(path);
            }
            return UnityEngine.Object.Instantiate(gameObject);
        }

        public Resource() { }
        public Resource(string name, string path)
        {
            this.name = name;
            this.path = path;
        }
    }
}

