using System;
using System.Collections.Generic;

namespace Networking.Support
{
    public class TypeIdMap<T, U> where T : Attribute, IAttributeId<U>
    {
        private Dictionary<Type, U> typeIdMap = new Dictionary<Type, U>();
        public U GetTypeId(Type type)
        {
            try
            {
                return typeIdMap[type];
            }
            catch (KeyNotFoundException) { }
            T MyAttribute = (T)Attribute.GetCustomAttribute(type, typeof(T));
            typeIdMap.Add(type, MyAttribute.Id);
            return MyAttribute.Id;
        }
    }
}

