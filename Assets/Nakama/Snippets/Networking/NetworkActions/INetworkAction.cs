using System.IO;

public interface INetworkAction
{
    int Dispatch(NetworkObjectManager networkObjectManager);
    void Encode(BinaryWriter writer);
}
