using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(GenericDictionary<string, GameObject>))]
public class StringGameObjectDictionaryPropertyDrawer : PropertyDrawer
{
    static float lineHeight = EditorGUIUtility.singleLineHeight;
    static float vertSpace = EditorGUIUtility.standardVerticalSpacing;
    
    public override void OnGUI(Rect pos, SerializedProperty property, GUIContent label)
    {
        // Draw list.
        var list = property.FindPropertyRelative("list");

        string fieldName = ObjectNames.NicifyVariableName(fieldInfo.Name);
        var currentPos = new Rect(lineHeight, pos.y + lineHeight, pos.width, lineHeight);
        EditorGUI.PropertyField(currentPos, list, new GUIContent(fieldName), true);

        // var listRect = new Rect(currentPos.x, currentPos.y + lineHeight, pos.width, lineHeight);
        // EditorGUI.PropertyField(listRect, keyArrayProperty, new GUIContent("key_list"),true);

        // EditorGUI.PropertyField(currentPos,)


        // EditorGUI.BeginChangeCheck();
        // if (EditorGUI.EndChangeCheck())
        // {
        //     // Debug.Log($"attribute = {attribute}");
        //     //方法1：直接设置为一样的名字
        //     foreach (var obj in list)
        //     {
        //         var p = obj as SerializedProperty;
        //         var key = p.FindPropertyRelative("Key");
        //         var value = p.FindPropertyRelative("Value");
        //         if (value.objectReferenceValue != null)
        //         {
        //             key.stringValue = value.objectReferenceValue.name;
        //         }
        //     }
        //     //方法2 有变化才修改
        // }

        //遍历，如果是key为空，value不为空，key值自动填充

        // Draw key collision warning.
        var keyCollision = property.FindPropertyRelative("keyCollision").boolValue;
        if (keyCollision)
        {
            currentPos.y += EditorGUI.GetPropertyHeight(list, true) + vertSpace;
            var entryPos = new Rect(lineHeight, currentPos.y, pos.width, lineHeight);
            EditorGUI.HelpBox(entryPos, "Duplicate keys will not be serialized.", MessageType.Warning);
        }

        // var buttonRect = new Rect(pos.width / 2 - 100, pos.y, 200, lineHeight);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float totHeight = 0f;

        // Height of KeyValue list.
        var listProp = property.FindPropertyRelative("list");
        totHeight += EditorGUI.GetPropertyHeight(listProp, true);

        // Height of key collision warning.
        bool keyCollision = property.FindPropertyRelative("keyCollision").boolValue;
        if (keyCollision)
        {
            totHeight += lineHeight * 2f + vertSpace;
        }

        return totHeight + lineHeight;
    }
}

[CustomPropertyDrawer(typeof(GenericDictionary<string, GameObject>.KeyValuePair))]
public class KeyValuePairPropertyDrawer : PropertyDrawer
{
    static float lineHeight = EditorGUIUtility.singleLineHeight;
    static float vertSpace = EditorGUIUtility.standardVerticalSpacing;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var currentPos = new Rect(lineHeight, position.y, position.width, lineHeight);
        var keyPos = new Rect(lineHeight, position.y, position.width * 0.45f, lineHeight);
        var valPos = new Rect(lineHeight + position.width * 0.55f, position.y, position.width * 0.45f, lineHeight);
        var key = property.FindPropertyRelative("Key");
        var val = property.FindPropertyRelative("Value");
        EditorGUI.PropertyField(keyPos, key, new GUIContent(""), true);
        EditorGUI.PropertyField(valPos, val, new GUIContent(""), true);
    }
}