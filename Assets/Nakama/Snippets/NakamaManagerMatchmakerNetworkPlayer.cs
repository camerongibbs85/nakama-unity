using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Nakama.TinyJson;
using System.Text;

namespace Nakama.Snippets
{
    public class NakamaManagerMatchmakerNetworkPlayer : MonoBehaviour
    {        
        IClient _client = null;
        ISocket _socket = null;
        ISession _session = null;
        IApiAccount _account = null;

        public bool Looping = true;
        public bool Running = true;
        public string objectNameBase = "o";
        public int objectLimit = 100;
        public int moves = 60;
        public int movePerionMs = 33;
        bool _hasRun = false;
        void OnDisable() {
            _hasRun = false;
        }
        private async void Update() {
            if(_hasRun) return;
            _hasRun = true;

            NakamaManager.Host = "desktop-itx";
            _client = NakamaManager.Instance.Client;
            _socket = NakamaManager.Instance.Socket;

            _session = await NakamaManager.Instance.Session;
            Debug.LogFormat("Active Session: {0}", _session);

            _account = await NakamaManager.Instance.Client.GetAccountAsync(_session);
            Debug.LogFormat("Account id: {0}", _account.User.Id);

            // NOTE As an example create a second user and _socket to matchmake against.
            IMatch match = null;
            _socket.ReceivedMatchmakerMatched += async matched =>
            {
                match = await _socket.JoinMatchAsync(matched);
            };
            await _socket.ConnectAsync(_session);
            await _socket.AddMatchmakerAsync("*", 2, 2);
            while(match == null)
            {
                await Task.Delay(System.TimeSpan.FromMilliseconds(100));
            }

            do
            {
                await Task.Delay(System.TimeSpan.FromSeconds(1)); // start sending after some time
                if(Running) await RunRoutine(match, _socket);
            }
            while(Looping);

            await Task.Delay(System.TimeSpan.FromSeconds(3)); // disconnect after some time.
            Debug.Log("After delay _socket closed.");
            await _socket.CloseAsync();
        }

        async Task RunRoutine(IMatch match, ISocket _socket)
        {
            int objectCount = 0;
            string[] objectNames = new string[objectLimit];
            GameObject[] objects = new GameObject[objectLimit];
            List<string> frameData = new List<string>();
            List<string>[] objectDatas = new List<string>[objectLimit];
            for (objectCount = 0; objectCount < objectLimit && Running; )
            {
                objectNames[objectCount] = objectNameBase + objectCount;
                PrimitiveType objectType = (PrimitiveType)Random.Range((int)0, (int)PrimitiveType.Cube + 1);
                objects[objectCount] = GameObject.CreatePrimitive(objectType);
                objects[objectCount].transform.SetParent(transform, false);
                objectDatas[objectCount] = new List<string>();

                var objectTypeBytes = System.BitConverter.GetBytes((short)objectType);
                var objectType64 = System.Convert.ToBase64String(objectTypeBytes);
                objectDatas[objectCount].Add(NetworkConstants.Create + "|" + objectType64);

                objectCount++;
                Debug.Log($"ObjectCount[{objectCount}]");
                
                for (int move = 0; move < moves && Running; move++)
                {
                    for (int j = 0; j < objectCount; j++)
                    {
                        Vector3 position =  Random.onUnitSphere * 3;
                        objects[j].transform.localPosition = position;
                        byte[] positionBytes = new byte[12];
                        System.Array.Copy(System.BitConverter.GetBytes(position.x), 0, positionBytes, 0, 4);
                        System.Array.Copy(System.BitConverter.GetBytes(position.y), 0, positionBytes, 4, 4);
                        System.Array.Copy(System.BitConverter.GetBytes(position.z), 0, positionBytes, 8, 4);
                        var position64 = System.Convert.ToBase64String(positionBytes);
                        objectDatas[j].Add(NetworkConstants.Position + "|" + position64);
                    }
                    await sendObjectData();
                    await Task.Delay(System.TimeSpan.FromMilliseconds(movePerionMs));
                }
            }

            for (int i = 0; i < objectCount; i++)
            {
                GameObject.Destroy(objects[i]);
                objectDatas[i].Add(NetworkConstants.Destroy + "|" + "Y");
            }

            await sendObjectData();

            for (int i = 0; i < objectCount; i++)
            {
                objectDatas[i] = null;
            }
            objectDatas = null;
            frameData.Clear();
            frameData = null;

            async Task sendObjectData()
            {
                for (int i = 0; i < objectCount; i++)
                {
                    var objectString = string.Join("#", objectDatas[i]);
                    frameData.Add(objectNames[i] + "}" + objectString);
                }
                var frameDataJson = string.Join("{", frameData);
                await _socket.SendMatchStateAsync(match.Id, 0, frameDataJson);
                for (int i = 0; i < objectCount; i++)
                {
                    objectDatas[i].Clear();
                }
                frameData.Clear();
            }
        }
    }
}