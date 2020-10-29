using System;

namespace Networking.Support
{
    public class ScriptableInterfaceListAttribute : ScriptableInterfaceAttribute
    {
        public ScriptableInterfaceListAttribute(Type interfaceType) : base(interfaceType) { }
    }
}

