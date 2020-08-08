/**
 * Copyright 2019 The Nakama Authors
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Nakama.Snippets
{
    public class NakamaManagerMatchmakerWithRelayedMultiplayer : MonoBehaviour
    {
        IClient _client = null;
        ISocket _socket = null;
        ISession _session = null;
        IApiAccount _account = null;
        IUserPresence _self = null;
        List<IUserPresence> _connectedOpponents = new List<IUserPresence>(2);

        bool _hasRun = false;
        void OnDisable() {
            _hasRun = false;
        }
        private async void Update() {
            lock(Lock)
            {
                foreach (var mainThreadAction in mainThreadActions)
                {
                    try { 
                        mainThreadAction(this);
                    } catch(Exception e) {
                        Debug.LogException(e);
                    }
                }
                mainThreadActions.Clear();
            }

            if(_hasRun) return;
            _hasRun = true;

            _client = NakamaManager.Instance.Client;
            _socket = NakamaManager.Instance.Socket;

            _session = await NakamaManager.Instance.Session;
            Debug.LogFormat("Active Session: {0}", _session);

            _account = await NakamaManager.Instance.Client.GetAccountAsync(_session);
            Debug.LogFormat("Account id: {0}", _account.User.Id);

            _socket.ReceivedError += Debug.LogError;
            _socket.Closed += () => Debug.Log("Socket closed.");
            _socket.Connected += () => Debug.Log("Socket connected.");
            _socket.ReceivedMatchmakerMatched +=  ReceivedMatchmakerMatched;
            _socket.ReceivedMatchPresence += ReceivedMatchPresence;
            _socket.ReceivedMatchState += ReceivedMatchState;

            await _socket.ConnectAsync(_session);
            Debug.Log("After _socket connected.");

            await _socket.AddMatchmakerAsync("*", 2, 2);
        }

        async void ReceivedMatchmakerMatched(IMatchmakerMatched matched)
        {
            Debug.LogFormat("Matched result: {0}", matched);
            var match = await _socket.JoinMatchAsync(matched);

            _connectedOpponents.AddRange(match.Presences);

            _self = match.Self;
            Debug.LogFormat("_Self: {0}", _self);
        }

        void ReceivedMatchPresence(IMatchPresenceEvent presenceEvent)
        {
            foreach (var presence in presenceEvent.Leaves)
            {
                _connectedOpponents.Remove(presence);
            }

            _connectedOpponents.AddRange(presenceEvent.Joins);
            
            // Remove _yourself from connected opponents.
            _connectedOpponents.Remove(_self);
            
            Debug.LogFormat("Connected opponents: [\n{0}\n]", string.Join(",\n  ", _connectedOpponents));
        }

        private static readonly object Lock = new object();
        List<Action<NakamaManagerMatchmakerWithRelayedMultiplayer>> mainThreadActions = new List<Action<NakamaManagerMatchmakerWithRelayedMultiplayer>>();
        Dictionary<string, GameObject> objects = new Dictionary<string, GameObject>();
        Dictionary<string, Action<string, string, string, NakamaManagerMatchmakerWithRelayedMultiplayer>> actions = new Dictionary<string, Action<string, string, string, NakamaManagerMatchmakerWithRelayedMultiplayer>>()
        {
            { NetworkConstants.Create, CreateAction },
            { NetworkConstants.Position, PositionAction },
            { NetworkConstants.Destroy, DestroyAction },
        };

        static void CreateAction(string objectKey, string actionKey, string actionValue, NakamaManagerMatchmakerWithRelayedMultiplayer host)
        {
            var bytes = System.Convert.FromBase64String(actionValue);
            PrimitiveType objectType = (PrimitiveType)System.BitConverter.ToInt16(bytes, 0);
            // host.mainThreadActions.Add(() => 
            // {
                host.objects[objectKey] = GameObject.CreatePrimitive(objectType);
                host.objects[objectKey].transform.SetParent(host.transform, false);
            // });
        }

        static void PositionAction(string objectKey, string actionKey, string actionValue, NakamaManagerMatchmakerWithRelayedMultiplayer host)
        {
            var bytes = System.Convert.FromBase64String(actionValue);
            var x = System.BitConverter.ToSingle(bytes, 0);
            var y = System.BitConverter.ToSingle(bytes, 4);
            var z = System.BitConverter.ToSingle(bytes, 8);
            // Vector3 position = JsonUtility.FromJson<Vector3>(actionValue);
            // host.mainThreadActions.Add(() =>
            // {
            Vector3 vector3 = new Vector3(x, y, z);
            host.objects[objectKey].transform.localPosition = vector3;
            // });
        }

        static void DestroyAction(string objectKey, string actionKey, string actionValue, NakamaManagerMatchmakerWithRelayedMultiplayer host)
        {
            // host.mainThreadActions.Add(() =>
            // {
                GameObject.Destroy(host.objects[objectKey]);
                host.objects.Remove(objectKey);
            // });
        }

        void ReceivedMatchState(IMatchState state)
        {
            string frameString = Encoding.UTF8.GetString(state.State);
            var objectDatas = frameString.Split('{');
            foreach (var objectData in objectDatas)
            {
                var objectDataSplit = objectData.Split('}');
                var objectKey = objectDataSplit[0];
                var objectActions = objectDataSplit[1].Split('#');
                foreach (var objectAction in objectActions)
                {
                    string[] actionSplit = objectAction.Split('|');
                    string actionKey = actionSplit[0];
                    string actionValue = actionSplit.Length > 1 ? actionSplit[1] : null;
                    Action<string, string, string, NakamaManagerMatchmakerWithRelayedMultiplayer> action;
                    if (actions.TryGetValue(actionKey, out action))
                    {
                        lock (Lock)
                        {
                            mainThreadActions.Add((host) =>
                                action.Invoke(objectKey, actionKey, actionValue, host)
                            );
                        }
                    }
                }
            }
        }

        void DecodeIfNotEmptyThen(string json, Action<Dictionary<string, string>> then)
        {
            if(!string.IsNullOrEmpty(json))
            {
                var data = TinyJson.JsonParser.FromJson<Dictionary<string, string>>(json);
                then(data);
            }
        }
    }
}
