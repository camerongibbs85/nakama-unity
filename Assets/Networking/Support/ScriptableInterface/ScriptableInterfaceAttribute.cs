using UnityEngine;
using System;
using System.Collections.Generic;

namespace Networking.Support
{
    public partial class ScriptableInterfaceAttribute : PropertyAttribute
    {
        public readonly Type InterfaceType;
        public readonly List<Type> Types;
        public readonly string[] Options;
        public ScriptableInterfaceAttribute(Type interfaceType)
        {
            InterfaceType = interfaceType;
            Types = ReflectiveEnumerator.GetScriptableObjectsOfInterface(interfaceType);
            Options = Types.ConvertAll(type => type.Name).ToArray();
        }
    }
}

