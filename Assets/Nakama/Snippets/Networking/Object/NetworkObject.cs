using UnityEngine;

public partial class NetworkObject
{
    public const int NullInstanceId = int.MinValue;
    private readonly GameObject gameObject;
    private readonly int instanceId;
    private NetworkObject(int instanceId, GameObject gameObject)
    {
        this.gameObject = gameObject;
        this.instanceId = instanceId;
    }

    public void SetParent(NetworkObject parentTransform, bool worldPositionStays)
    {
        gameObject.transform.SetParent(parentTransform.gameObject.transform, worldPositionStays);
    }

    public void SetLocalPosition(Vector3 position)
    {
        gameObject.transform.localPosition = position;
    }

    public int Destroy()
    {
        GameObject.Destroy(gameObject);
        return instanceId;
    }
}
