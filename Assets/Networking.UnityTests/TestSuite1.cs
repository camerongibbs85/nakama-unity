using System.Collections;
using System.Collections.Generic;
using Networking.ObjectCreator;
using Networking.Support;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Networking.UnityTests
{
    public class TestSuite1
    {
        static Queue<Object> unityDestroyables = new Queue<Object>();

        [SetUp]
        public void SetUp()
        {
        }

        [TearDown]
        public void TearDown()
        {
            Object obj;
            while (unityDestroyables.Count > 0 && (obj = unityDestroyables.Dequeue()))
            {
                Object.Destroy(obj);
            }
        }

        [Test]
        public void BuildPathToParent_When_DirectChildOfRoot_Then_ChildName()
        {
            var root = new GameObject("root");
            unityDestroyables.Enqueue(root);
            var child = new GameObject("child");
            unityDestroyables.Enqueue(child);
            child.transform.SetParent(root.transform, false);
            var path = HierarchyUtils.BuildPathToParent(child.transform, root.transform);
            Assert.AreEqual("child", path);
        }

        [Test]
        public void BuildPathToParent_When_NthChildOfRoot_Then_NChildrenBeforeChildName()
        {
            var root = new GameObject("root");
            unityDestroyables.Enqueue(root);
            string expectedPath = null;
            GameObject nthChild = null;
            string childName = null;
            var n = 5;
            for (int i = 0; i < n; i++)
            {
                childName = "child" + i;
                expectedPath += childName + HierarchyUtils.PathSeparator;
                var newChild = new GameObject(childName);
                newChild.transform.SetParent(nthChild ? nthChild.transform : root.transform);
                unityDestroyables.Enqueue(newChild);
                nthChild = newChild;
            }
            expectedPath += "child";
            var child = new GameObject("child");
            child.transform.SetParent(nthChild.transform, false);
            unityDestroyables.Enqueue(child);

            var path = HierarchyUtils.BuildPathToParent(child.transform, root.transform);
            Assert.AreEqual(expectedPath, path);
        }

        [Test]
        public void GetChildFromParentPath_When_DirectChildOfRoot_Then_ReturnsChildTransform()
        {
            var root = new GameObject("root");
            unityDestroyables.Enqueue(root);
            var child = new GameObject("child");
            unityDestroyables.Enqueue(child);
            child.transform.SetParent(root.transform, false);
            var test = HierarchyUtils.GetChildFromParentPath(root.transform, "child");
            UnityEngine.Assertions.Assert.AreEqual(child, test.gameObject);
        }

        [Test]
        public void GetChildFromParentPath_When_DirectChildOfRoot_Then_BuildPathToParent_ReturnsChildTransform()
        {
            var root = new GameObject("root");
            unityDestroyables.Enqueue(root);
            var child = new GameObject("child");
            unityDestroyables.Enqueue(child);
            child.transform.SetParent(root.transform, false);
            var path = HierarchyUtils.BuildPathToParent(child.transform, root.transform);
            var test = HierarchyUtils.GetChildFromParentPath(root.transform, path);
            UnityEngine.Assertions.Assert.AreEqual(child, test.gameObject);
        }

        [Test]
        public void GetChildFromParentPath_When_NthChildOfRoot_Then_BuildPathToParent_ReturnsChildTransform()
        {
            var root = new GameObject("root");
            unityDestroyables.Enqueue(root);
            string expectedPath = null;
            GameObject nthChild = null;
            string childName = null;
            var n = 5;
            for (int i = 0; i < n; i++)
            {
                childName = "child" + i;
                expectedPath += childName + HierarchyUtils.PathSeparator;
                var newChild = new GameObject(childName);
                newChild.transform.SetParent(nthChild ? nthChild.transform : root.transform);
                unityDestroyables.Enqueue(newChild);
                nthChild = newChild;
            }
            expectedPath += "child";
            var child = new GameObject("child");
            child.transform.SetParent(nthChild.transform, false);
            unityDestroyables.Enqueue(child);

            var path = HierarchyUtils.BuildPathToParent(child.transform, root.transform);
            var test = HierarchyUtils.GetChildFromParentPath(root.transform, path);
            UnityEngine.Assertions.Assert.AreEqual(child, test.gameObject);
        }

        [Test]
        public void PrimitiveCreator_Creates_Expected_PrimitiveType()
        {
            PrimitiveCreator primitiveCreator = ScriptableObject.CreateInstance<PrimitiveCreator>();
            var (id, bytes) = primitiveCreator.Create(PrimitiveType.Sphere);
            var type = bytes[0];

            UnityEngine.Assertions.Assert.AreEqual(type, (byte)PrimitiveType.Sphere);
        }
    }
}

