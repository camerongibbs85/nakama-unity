using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;
using System.Collections.Generic;
using System;
using Networking.Support;

namespace Networking.Support
{
    [CustomPropertyDrawer(typeof(ScriptableInterfaceListAttribute), true)]
    public class ScriptableInterfaceListDrawer : PropertyDrawer
    {
        ReorderableList list;
        List<ScriptableInterfaceData> indices;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return list == null ? EditorGUI.GetPropertyHeight(property) : list.GetHeight();
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            ScriptableInterfaceListAttribute myAttribute = attribute as ScriptableInterfaceListAttribute;

            var items = property.FindPropertyRelative("items");
            if (items == null) { EditorGUI.PropertyField(position, property, label); return; }

            var baseItem = fieldInfo.GetValue(property.serializedObject.targetObject);
            var itemsInfo = baseItem.GetType().GetField("items");
            var itemsList = itemsInfo.GetValue(baseItem) as IList;
            if (list == null)
            {
                indices = new List<ScriptableInterfaceData>(items.arraySize);
                while (indices.Count < items.arraySize)
                {
                    indices.Add(new ScriptableInterfaceData());
                }
                list = new ReorderableList(itemsList, itemsInfo.FieldType.GenericTypeArguments[0]);
                RegisterDrawElement(myAttribute, items);
                RegisterElementHeight(items);
                RegisterAdd(items);
                RegisterRemove(items);
                RegisterReorder();
            }
            list.DoList(position);

            EditorGUI.EndProperty();
        }

        private void RegisterReorder()
        {
            list.onReorderCallbackWithDetails += (ReorderableList list, int oldIndex, int newIndex) =>
            {
                if (oldIndex == newIndex) return;

                int start, end, offset;
            // moving up the list, all others move down one, copy from next value
            if (oldIndex < newIndex) { start = oldIndex; end = newIndex; offset = 1; }
            // moving down the list, all others move up one, copy from previous value
            else { start = oldIndex; end = newIndex; offset = -1; }

            // store old index
            var oldValue = indices[oldIndex];
            // copy all moved object indeces from their old spot
            for (int i = start; i != end; i += offset)
                {
                    indices[i] = indices[i + offset];
                }
            // restore old index to new pos
            indices[newIndex] = oldValue;

            };
        }

        private void RegisterRemove(SerializedProperty items)
        {
            list.onRemoveCallback += (ReorderableList list) =>
            {
                var index = list.index;
                if (index > -1)
                {
                    var element = items.GetArrayElementAtIndex(index);
                    var objRev = element.objectReferenceValue;
                    items.DeleteArrayElementAtIndex(index);
                    if (objRev)
                    {
                        UnityEngine.Object.DestroyImmediate(objRev);
                    }
                    else
                    {
                        indices.RemoveAt(index);
                        while (index >= items.arraySize)
                        {
                            index--;
                        }
                        list.index = index;
                    }
                }
            };
        }

        private void RegisterAdd(SerializedProperty items)
        {
            list.onAddCallback += (ReorderableList list) =>
            {
            // var index = list.index;
            // if(index == -1) 
            var index = items.arraySize;
                if (index > -1)
                {
                    items.InsertArrayElementAtIndex(index);
                    var element = items.GetArrayElementAtIndex(index);
                    element.objectReferenceValue = null;
                    indices.Insert(index, new ScriptableInterfaceData());
                    list.index = index;
                }
            };
        }

        private void RegisterElementHeight(SerializedProperty items)
        {
            list.elementHeightCallback += (int index) =>
            {
                var element = items.GetArrayElementAtIndex(index);
                var elementLabel = new GUIContent(index.ToString());
                return ScriptableInterfaceDrawer.DoGetPropertyHeight(element, elementLabel, indices[index]);
            };
        }

        private void RegisterDrawElement(ScriptableInterfaceListAttribute attribute, SerializedProperty items)
        {
            list.drawElementCallback += (rect, index, isActive, isFocused) =>
            {
                while (indices.Count <= index)
                {
                    indices.Add(new ScriptableInterfaceData());
                }
                var element = items.GetArrayElementAtIndex(index);
                var elementLabel = new GUIContent(index.ToString());
                ScriptableInterfaceDrawer.DoOnGUI(rect, element, elementLabel, indices[index], attribute);
            };
        }
    }
}