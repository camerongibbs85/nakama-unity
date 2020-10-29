namespace Networking.NetworkActions
{
    public enum NetworkActionId : byte
    {
        CreateActions = 0,
        CreatePrimitive,
        NetworkedCreator,
        Destroy,

        TransformActions = 20,
        Parent,
        Postition,
        Rotation,
        Scale,

        ChildActions = 30,
        RegisterChild,

        NetworkActionEnd
    }
}

