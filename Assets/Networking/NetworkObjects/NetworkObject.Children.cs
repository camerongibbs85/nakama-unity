using Networking.Support;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Networking.NetworkObjects
{
    public partial class NetworkObject
    {
        public class Children
        {
            // the child could be registered locally or remotely or both
            // store all id's that reference a path
            Dictionary<int, string> childIdPaths = new Dictionary<int, string>();
            // each child can have only one path
            // Id -> Path -> GameObject
            Dictionary<string, GameObject> childPathGameObjects = new Dictionary<string, GameObject>();

            NetworkObject networkObject;

            public Children(NetworkObject networkObject)
            {
                this.networkObject = networkObject;
            }

            public void RegisterChild(int id, string path)
            {
                try
                {
                    var gameObject = HierarchyUtils.GetChildFromParentPath(networkObject.gameObject.transform, path).gameObject;
                    try
                    {
                        childPathGameObjects.Add(path, gameObject);
                    }
                    catch (ArgumentException) { }
                    try
                    {
                        childIdPaths.Add(id, path);
                        childIdPaths.Add(gameObject.GetInstanceID(), path);
                    }
                    catch (ArgumentException) { }
                }
                catch (NullReferenceException) { }
            }

            public GameObject GetChild(int id)
            {
                try
                {
                    return childPathGameObjects[childIdPaths[id]];
                }
                catch (KeyNotFoundException)
                {
                    return null;
                }
            }
        }
    }
}

