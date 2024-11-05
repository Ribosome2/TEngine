using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BindObjectMono))]
public class BindObjectMonoEditor : Editor
{
    private static bool _includeHideObj = true;
    private static bool _isExportDeclare = true;

    private const string Tip1 = "是否包含隐藏物体[√]已包含";
    private const string Tip2 = "是否包含隐藏物体[X]未包含";
    private const string ButtonTip = "重新生成，m_开头的控件会被添加进字典";

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        BindObjectMono myScript = (BindObjectMono) target;
        EditorGUILayout.Space();
        _includeHideObj = GUILayout.Toggle(_includeHideObj, _includeHideObj ? Tip1 : Tip2);

        EditorGUILayout.Space();
        _isExportDeclare = GUILayout.Toggle(_isExportDeclare, "生成脚本声明");

        EditorGUILayout.Space();
        GUILayoutOption[] options = { GUILayout.Width(Screen.width - 50), GUILayout.Height(30) };
        if (GUILayout.Button(ButtonTip, new GUIStyle("LargeButton"), options))
        {
            myScript.BindObject(_includeHideObj);

            if (_isExportDeclare)
                ExportDeclare(myScript);
        }

        if (!string.IsNullOrEmpty(myScript.ViewScripts))
        {
            if (GUILayout.Button("打开代码"))
            {
                AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<Object>(myScript.ViewScripts));
            }
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }

    private void ExportDeclare(BindObjectMono target)
    {
        var viewScripts = target.ViewScripts;
        if (string.IsNullOrEmpty(viewScripts))
            return;

        KyleUICodeGenWnd.GenerateCodeDeclare(target.gameObject, viewScripts);
    }

    public static void AbsolutePathToRelativePath(List<string> paths)
    {
        for (int i = 0; i < paths.Count; ++i)
        {
            paths[i] = AbsolutePathToRelativePath(paths[i]);
        }
    }

    public static string AbsolutePathToRelativePath(string path)
    {
        if (path.StartsWith("Assets"))
            return path;

        return Regex.Replace(path, ".+?(?=Assets)", "");
    }
}
