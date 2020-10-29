using System.Threading.Tasks;
using UnityEngine;
using static Networking.NetworkObjects.NetworkObject;
using Networking.ObjectCreator;

namespace Networking.Nakama
{
    public class NakamaNetworkObjectManagerRoutine : MonoBehaviour
    {
        public bool Looping = true;
        public bool Running = true;
        public int ObjectLimit = 100;
        public int Moves = 60;

        [Range(10, 1000)]
        public int movePerionMs = 33;
        bool _hasRun = false;
        NakamaNetworkedManager nakama;
        void OnDisable()
        {
            _hasRun = false;
        }
        private async void Update()
        {
            if (_hasRun) return;
            _hasRun = true;

            nakama = GetComponent<NakamaNetworkedManager>();

            do
            {
                await Task.Delay(System.TimeSpan.FromSeconds(1)); // start sending after some time
                if (Running) await RunRoutine(ObjectLimit, Moves);
            }
            while (Looping);
        }

        async Task RunRoutine(int objectLimit, int moves)
        {
            int objectCount = 0;
            var objects = new Proxy[objectLimit];
            for (objectCount = 0; objectCount < objectLimit && Running;)
            {
                PrimitiveType objectType = (PrimitiveType)Random.Range((int)0, (int)PrimitiveType.Cube + 1);
                objects[objectCount] = nakama.NetworkObjectManager.Create<PrimitiveCreator>((creator) => creator.Create(objectType));
                objectCount++;
                Debug.Log($"ObjectCount[{objectCount}]");

                for (int move = 0; move < moves && Running; move++)
                {
                    for (int j = 0; j < objectCount; j++)
                    {
                        Vector3 position = Random.onUnitSphere * 3;
                        objects[j].SetLocalPosition(position);
                    }
                    await Task.Delay(System.TimeSpan.FromMilliseconds(movePerionMs));
                }
            }

            for (int i = 0; i < objectCount; i++)
            {
                objects[i].Destroy();
            }
        }
    }
}