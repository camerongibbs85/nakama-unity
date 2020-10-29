using Nakama;
using Nakama.Snippets;
using System;
using System.Collections.Generic;
using UnityEngine;
using Networking.Support;
using Networking.Manager;

namespace Networking.Nakama
{
    public class NakamaNetworkedManager : MonoBehaviour
    {
        IClient _client = null;
        ISocket _socket = null;
        ISession _session = null;
        IApiAccount _account = null;
        IUserPresence _self = null;
        IMatch _match = null;
        List<IUserPresence> _connectedOpponents = new List<IUserPresence>(2);
        private static readonly object Lock = new object();
        private readonly Queue<byte[]> payloads = new Queue<byte[]>();
        public INetworkObjectHandler NetworkObjectHandler { get; private set; }

        [SerializeField]
        [ScriptableInterface(typeof(INetworkObjectManager))]
        protected ScriptableObject networkObjectManager = null;
        public INetworkObjectManager NetworkObjectManager
        {
            get { return networkObjectManager as INetworkObjectManager; }
        }

        public event Action OnJoinedMatch;

        bool _hasRun = false;
        void OnDisable()
        {
            _hasRun = false;
        }
        private async void Update()
        {
            if (_hasRun) return;
            _hasRun = true;

            NetworkObjectManager.ParentTransform = transform;
            NetworkObjectHandler = new NetworkObjectHandler(NetworkObjectManager);

            //NakamaManager.Host = "desktop-itx";
            _client = NakamaManager.Instance.Client;
            _socket = NakamaManager.Instance.Socket;

            _session = await NakamaManager.Instance.Session;
            Debug.LogFormat("Active Session: {0}", _session);

            _account = await NakamaManager.Instance.Client.GetAccountAsync(_session);
            Debug.LogFormat("Account id: {0}", _account.User.Id);

            _socket.ReceivedError += Debug.LogError;
            _socket.Closed += () => Debug.Log("Socket closed.");
            _socket.Connected += () => Debug.Log("Socket connected.");
            _socket.ReceivedMatchmakerMatched += ReceivedMatchmakerMatched;
            _socket.ReceivedMatchPresence += ReceivedMatchPresence;
            _socket.ReceivedMatchState += ReceivedMatchState;

            await _socket.ConnectAsync(_session);
            Debug.Log("After _socket connected.");

            await _socket.AddMatchmakerAsync("*", 2, 2);
        }

        private void LateUpdate()
        {
            if (_match != null)
            {
                SendPayloads();
                ReceivePayloads();
            }
        }

        private void ReceivePayloads()
        {
            while (payloads.Count > 0)
            {
                byte[] payload;
                lock (Lock)
                {
                    payload = payloads.Dequeue();
                }
                NetworkObjectHandler.HandleNetworkPayload(payload);
            }
        }

        private void SendPayloads()
        {
            var payload = NetworkObjectHandler.GetNetworkPayload();
            if (payload != null && payload.Length > 0)
            {
                _socket.SendMatchStateAsync(_match.Id, 0, payload);
            }
        }

        async void ReceivedMatchmakerMatched(IMatchmakerMatched matched)
        {
            Debug.LogFormat("Matched result: {0}", matched);
            _match = await _socket.JoinMatchAsync(matched);

            _connectedOpponents.AddRange(_match.Presences);

            _self = _match.Self;
            Debug.LogFormat("_Self: {0}", _self);

            OnJoinedMatch();
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

        void ReceivedMatchState(IMatchState state)
        {
            // Debug.Log($"Received match state {state.UserPresence.UserId}");
            lock (Lock)
            {
                payloads.Enqueue(state.State);
            }
        }
    }
}