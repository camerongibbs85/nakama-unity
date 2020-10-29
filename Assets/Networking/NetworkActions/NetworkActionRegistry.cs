using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Networking.NetworkActions
{
    public delegate INetworkAction NetworkActionCreateFunction();
    public delegate NetworkActionId NetworkActionGetIdFunction();

// in the editor - add this attribute so that registration gets logged after load / recomplie for debugging purposes
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public static class NetworkActionRegistry
    {
        private static readonly Dictionary<NetworkActionId, NetworkActionCreateFunction> networkActionCreateFunctions;

        static NetworkActionRegistry()
        {
            networkActionCreateFunctions = new Dictionary<NetworkActionId, NetworkActionCreateFunction>();
            var thisType = MethodBase.GetCurrentMethod().DeclaringType;
            var iNetworkActionType = typeof(INetworkAction);
            var networkActionTypes = Assembly.GetAssembly(iNetworkActionType)
                .GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && iNetworkActionType.IsAssignableFrom(myType)).ToList();

            MethodInfo getIdMethod = typeof(NetworkActionExtensions).GetMethod("GetId");
            MethodInfo createMethod = typeof(NetworkActionExtensions).GetMethod("Create");
            //MethodInfo getIdDelegate = typeof(NetworkActionGetIdFunctionGeneric).GetMethodInfo();

            foreach (var networkActionType in networkActionTypes)
            {
                NetworkActionId getId = NetworkActionId.NetworkActionEnd;
                NetworkActionCreateFunction create = null;
                try
                {
                    MethodInfo getIdGeneric = getIdMethod.MakeGenericMethod(new[] { networkActionType });
                    MethodInfo createGeneric = createMethod.MakeGenericMethod(new[] { networkActionType });
                    getId = ((NetworkActionGetIdFunction)Delegate.CreateDelegate(typeof(NetworkActionGetIdFunction), null, getIdGeneric))();
                    create = (NetworkActionCreateFunction)Delegate.CreateDelegate(typeof(NetworkActionCreateFunction), null, createGeneric);
                    Debug.Assert(getId != NetworkActionId.NetworkActionEnd);
                    Debug.Assert(create != null);
                }
                catch (Exception) { }
                networkActionCreateFunctions.Add(getId, create);
                Debug.Log($"[{thisType.Name}] Registered id[{getId}] and Create function for NetworkAction[{networkActionType.Name}]");
            }
        }

        public static bool TryGetValue(NetworkActionId networkActionId, out NetworkActionCreateFunction networkActionCreateFunction)
        {
            return networkActionCreateFunctions.TryGetValue(networkActionId, out networkActionCreateFunction);
        }
    }
}

