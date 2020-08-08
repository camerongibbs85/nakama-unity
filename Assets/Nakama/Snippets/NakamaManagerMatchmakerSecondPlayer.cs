using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Nakama.TinyJson;
using System.Text;

namespace Nakama.Snippets
{
    public static class NetworkConstants
    {
        public static string Create = "c";
        public static string Position = "p";
        public static string Destroy = "d";
    }
    public class NakamaManagerMatchmakerSecondPlayer : MonoBehaviour
    {
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

            var client = NakamaManager.Instance.Client;

            // NOTE As an example create a second user and socket to matchmake against.
            var deviceId = System.Guid.NewGuid().ToString();
            var session = await client.AuthenticateDeviceAsync(deviceId);
            var socket = client.NewSocket();
            IMatch match = null;
            socket.ReceivedMatchmakerMatched += async matched =>
            {
                match = await socket.JoinMatchAsync(matched);
            };
            await socket.ConnectAsync(session);
            await socket.AddMatchmakerAsync("*", 2, 2);
            while(match == null)
            {
                await Task.Delay(System.TimeSpan.FromMilliseconds(100));
            }

            do
            {
                await Task.Delay(System.TimeSpan.FromSeconds(1)); // start sending after some time
                if(Running) await RunRoutine(match, socket);
            }
            while(Looping);

            await Task.Delay(System.TimeSpan.FromSeconds(3)); // disconnect after some time.
            Debug.Log("After delay socket closed.");
            await socket.CloseAsync();
        }

        async Task RunRoutine(IMatch match, ISocket socket)
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
                await socket.SendMatchStateAsync(match.Id, 0, frameDataJson);
                for (int i = 0; i < objectCount; i++)
                {
                    objectDatas[i].Clear();
                }
                frameData.Clear();
            }
        }
    }
}