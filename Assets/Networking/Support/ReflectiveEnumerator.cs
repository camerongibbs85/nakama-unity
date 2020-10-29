using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Networking.Manager;

namespace Networking.Support
{
    public static class ReflectiveEnumerator
    {
        public static readonly List<Type> networkedManagerTypes;
        static ReflectiveEnumerator()
        {
            networkedManagerTypes = GetScriptableObjectsOfInterface<INetworkObjectManager>();
        }

        public static ScriptableObject Create(Type type)
        {
            return ScriptableObject.CreateInstance(type);
        }

        public static List<Type> GetScriptableObjectsOfInterface<T>()
        {
            return GetScriptableObjectsOfInterface(typeof(T));
        }

        public static List<Type> GetScriptableObjectsOfInterface(Type type)
        {
            var types = Assembly.GetAssembly(type).GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && type.IsAssignableFrom(myType) && myType.IsSubclassOf(typeof(ScriptableObject))).ToList();
            return types;
        }


        public static IEnumerable<T> GetEnumerableOfType<T>(params object[] constructorArgs) where T : class, IComparable<T>
        {
            List<T> objects = new List<T>();
            foreach (Type type in
                Assembly.GetAssembly(typeof(T)).GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T)) && myType.IsSubclassOf(typeof(ScriptableObject))))
            {
                objects.Add((T)Activator.CreateInstance(type, constructorArgs));
            }
            objects.Sort();
            return objects;
        }
    }
}

