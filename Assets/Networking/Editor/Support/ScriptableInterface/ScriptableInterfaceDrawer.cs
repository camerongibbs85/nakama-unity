using UnityEngine;
using UnityEditor;
using Networking.Support;

namespace Networking.Support
{
    [CustomPropertyDrawer(typeof(ScriptableInterfaceAttribute), true)]
    public class ScriptableInterfaceDrawer : PropertyDrawer
    {
        const string dialogDecisionKey = "ScriptableInterfaceDrawer.IgnoreReplaceDelete";
        ScriptableInterfaceData data = new ScriptableInterfaceData();
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return DoGetPropertyHeight(property, label, data);
        }
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            // First get the attribute since it contains the range for the slider
            ScriptableInterfaceAttribute myAttribute = attribute as ScriptableInterfaceAttribute;
            DoOnGUI(rect, property, label, data, myAttribute);
        }

        public static float DoGetPropertyHeight(SerializedProperty property, GUIContent label, ScriptableInterfaceData data)
        {
            var propertyHeight = EditorGUI.GetPropertyHeight(property);
            return propertyHeight * 2;
        }

        public static void DoOnGUI(Rect rect, SerializedProperty property, GUIContent label, ScriptableInterfaceData data, ScriptableInterfaceAttribute attribute)
        {
            var propertyHeight = EditorGUI.GetPropertyHeight(property);

            EditorGUI.BeginProperty(rect, label, property);

            if (data.index == -1) data.index = property.objectReferenceValue ? attribute.Types.IndexOf(property.objectReferenceValue.GetType()) : 0;

            var itemRect = new Rect(rect.x, rect.y, rect.width - 20, propertyHeight);
            DrawProperty(rect, property, label, data, propertyHeight, ref itemRect);
            DrawPropertyCreator(rect, property, data, attribute, propertyHeight, itemRect);

            EditorGUI.EndProperty();
        }

        private static void DrawPropertyCreator(Rect rect, SerializedProperty property, ScriptableInterfaceData data, ScriptableInterfaceAttribute attribute, float propertyHeight, Rect itemRect)
        {
            if (!data.confirming)
            {
                var popupRect = new Rect(rect.x, itemRect.yMax, rect.width - 20, propertyHeight);
                var buttonRect = new Rect(popupRect.xMax, itemRect.yMax, 20, propertyHeight);
                data.index = EditorGUI.Popup(popupRect, data.index, attribute.Options);
                if (GUI.Button(buttonRect, new GUIContent("+"), EditorStyles.miniButtonRight))
                {
                    if (property.objectReferenceValue) data.confirming = true;
                    else Replace(property, data.index, attribute);
                }
            }
            else
            {
                var messageRect = new Rect(rect.x, itemRect.yMax, rect.width - 40, propertyHeight);
                var buttonRect1 = new Rect(messageRect.xMax, itemRect.yMax, 20, propertyHeight);
                var buttonRect2 = new Rect(buttonRect1.xMax, itemRect.yMax, 20, propertyHeight);
                GUI.Label(messageRect, "Are you sure?", EditorStyles.popup);
                if (GUI.Button(buttonRect1, new GUIContent("Y"), EditorStyles.miniButtonRight))
                {
                    data.confirming = false;
                    Replace(property, data.index, attribute);
                }
                if (GUI.Button(buttonRect2, new GUIContent("N"), EditorStyles.miniButtonRight))
                {
                    data.confirming = false;
                }
            }
        }

        private static void DrawProperty(Rect rect, SerializedProperty property, GUIContent label, ScriptableInterfaceData data, float propertyHeight, ref Rect itemRect)
        {
            EditorGUI.PropertyField(itemRect, property, label);
            var inspectRect = new Rect(itemRect.xMax, rect.y, 20, propertyHeight);
            if (!data.inspecting)
            {
                var inspectContent = EditorGUIUtility.IconContent("CustomTool", "Show Item Inspector");
                if (GUI.Button(inspectRect, inspectContent) && property.objectReferenceValue)
                {
                    data.inspecting = true;
                    data.editor = Editor.CreateEditor(property.objectReferenceValue);
                }
            }
            else if (property.objectReferenceValue == null)
            {
                data.inspecting = false;
                data.editor = null;
            }
            else
            {
                var inspectContent = EditorGUIUtility.IconContent("Audio Mixer@2x", "Hide Item Inspector");
                if (GUI.Button(inspectRect, inspectContent))
                {
                    data.inspecting = false;
                    data.editor = null;
                }
                else
                {
                    data.editor.DrawHeader();
                    data.editor.DrawDefaultInspector();
                }
            }
        }

        private static void Replace(SerializedProperty property, int index, ScriptableInterfaceAttribute range)
        {
            if (property.objectReferenceValue) UnityEngine.Object.DestroyImmediate(property.objectReferenceValue);
            property.objectReferenceValue = ReflectiveEnumerator.Create(range.Types[index]);
        }
    }
}