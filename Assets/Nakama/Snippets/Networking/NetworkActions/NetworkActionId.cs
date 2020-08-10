public enum NetworkActionId : byte
{
    CreateActions = 0,
    CreatePrimitive,
    Destroy,

    TransformActions = 20,    
    Parent,
    Postition,
    Rotation,

    NetworkActionEnd
}
