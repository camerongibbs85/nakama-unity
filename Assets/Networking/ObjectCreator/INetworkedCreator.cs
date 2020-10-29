using System.IO;
using UnityEngine;

namespace Networking.ObjectCreator
{
    public interface INetworkedCreator
    {
        byte[] ExtractBytes(BinaryReader reader);
        GameObject UseBytes(byte[] bytes, Transform parentTransform);
    }
}

