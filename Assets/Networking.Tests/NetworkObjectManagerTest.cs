using UnityEngine;
using UnityEngine.Serialization;
using Networking.Support;
using Networking.Manager;

namespace Networking.Tests
{
    public abstract class NetworkObjectManagerTest : MonoBehaviour
    {
        [SerializeField]
        [ScriptableInterface(typeof(INetworkObjectManager))]
        [FormerlySerializedAs("actor")]
        protected ScriptableObject recorder = null;
        public INetworkObjectManager Recorder
        {
            get { return recorder as INetworkObjectManager; }
        }
        protected INetworkObjectHandler recorderHandler;

        [SerializeField]
        [ScriptableInterface(typeof(INetworkObjectManager))]
        [FormerlySerializedAs("reflector")]
        protected ScriptableObject replayer = null;
        public INetworkObjectManager Replayer
        {
            get { return replayer as INetworkObjectManager; }
        }
        protected INetworkObjectHandler replayerHandler;

        protected virtual void Setup(Transform recorderTransform, Transform replayerTransform)
        {
            Recorder.ParentTransform = recorderTransform;
            recorderHandler = new NetworkObjectHandler(Recorder);
            Replayer.ParentTransform = replayerTransform;
            replayerHandler = new NetworkObjectHandler(Replayer);
        }
    }
}