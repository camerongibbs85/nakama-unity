using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Networking.Manager;

namespace Networking.Support
{
    public static class HierarchyUtils
    {
        public const string PathSeparator = "/";
        private static Stack<string> TransformStack = new Stack<string>();
        public static string BuildPathToParent(Transform child, Transform parent)
        {
            TransformStack.Clear();
            var xFrom = child;
            do
            {
                TransformStack.Push(xFrom.name);
                xFrom = xFrom.parent;
            } while(xFrom != null && xFrom != parent);
            return string.Join(PathSeparator, TransformStack);
        }
        public static Transform GetChildFromParentPath(Transform parent, string path)
        {
            return parent.Find(path);
        }
    }
}

