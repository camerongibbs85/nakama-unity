using UnityEngine;
using UnityEditor;
using Networking.Support;

namespace Networking.Tests
{
    [CustomEditor(typeof(MonoUsingScriptableObject))]
    public class MonoUsingScriptableObjectEditor : Editor
    {
        string[] options = ReflectiveEnumerator.networkedManagerTypes.ConvertAll(type => type.Name).ToArray();
        int index;

        MonoUsingScriptableObject myObject;
        private void OnEnable()
        {
            myObject = target as MonoUsingScriptableObject;
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            index = EditorGUILayout.Popup(index, options);
            if (GUILayout.Button("Create"))
            {
                var type = ReflectiveEnumerator.networkedManagerTypes[index];
                if (myObject.manager) { DestroyImmediate(myObject.manager); }
                myObject.manager = ReflectiveEnumerator.Create(type);
            }
        }
    }
}

