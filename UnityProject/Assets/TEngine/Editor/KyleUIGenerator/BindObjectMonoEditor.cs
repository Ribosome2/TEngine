using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(BindObjectMono))]
public class BindObjectMonoEditor : Editor
{
    private static bool _includeHideObj = true;
    private static bool _isExportDeclare = true;

    private const string ButtonTip = "重新生成";

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        BindObjectMono myScript = (BindObjectMono) target;
        EditorGUILayout.Space();

        EditorGUILayout.Space();

        EditorGUILayout.Space();
        if (GUILayout.Button("替换为UIButtonSuper",GUILayout.Height(25)))
        {
            ReplaceUIButtonSuper(myScript.transform);
        }
        GUILayout.BeginHorizontal();
        if (GUILayout.Button(ButtonTip, GUILayout.Height(40),GUILayout.ExpandWidth(false)))
        {
            myScript.BindObject(_includeHideObj);

            if (_isExportDeclare)
                ExportDeclare(myScript);
        }

        if (!string.IsNullOrEmpty(myScript.ViewCode))
        {
            if (GUILayout.Button("打开代码",GUILayout.Height(40)))
            {
                AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<Object>(myScript.ViewCode));
            }
            
           
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if(GUILayout.Button("生成Windows代码"))
        {
            KyleUICodeGenWnd.Generate(myScript.gameObject,false);
        }
        
        if(GUILayout.Button("生成Cell代码"))
        {
            KyleUICodeGenWnd.Generate(myScript.gameObject,true);
        }
        GUILayout.EndHorizontal();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }

    private void ExportDeclare(BindObjectMono target)
    {
        var viewScripts = target.ViewCode;
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
    
    
    private void ReplaceUIButtonSuper(Transform root)
    {
        FindAndReplaceButton(root);
    }

    private void FindAndReplaceButton(Transform parent)
    {
        string defaultClickSound = "Assets/GameAssets/Audio/SoundEffect/ui/ui_click.mp3";
        Button[] btns = parent.gameObject.GetComponentsInChildren<Button>();
        for (int i = 0; i < btns.Length; ++i)
        {
            var btn = btns[i];
            if (btn is UIButtonSuper uiButtonSuper)
            {
                CheckDefaultClickSound(uiButtonSuper, defaultClickSound);
                continue;
            }

            Debug.Log($"Replace Button 2 UIButtonSuper ==>> {btn.name}");
            var go = btn.gameObject;
            var graphic = btn.targetGraphic;

            Undo.DestroyObjectImmediate(btn);

            UIButtonSuper newBtn = go.AddComponent<UIButtonSuper>();
            newBtn.targetGraphic = graphic;

            CheckDefaultClickSound(newBtn, defaultClickSound);
        }
    }
    private void CheckDefaultClickSound(UIButtonSuper newBtn, string defaultClickSound)
    {
        var sounds = newBtn.m_ButtonUISounds;
        bool isNoClickSound = true;
        for (int j = 0; j < sounds.Count; j++)
        {
            var sound = sounds[j];
            if (sound.ButtonSoundType == ButtonSoundType.Click)
            {
                if(!sound.ButtonUISoundName.Contains("Assets/GameAssets/Audio/"))
                    sound.ButtonUISoundName = defaultClickSound;
                isNoClickSound = false;
            }
        }

        if (isNoClickSound)
        {
            sounds.Add(new ButtonSoundCell { ButtonSoundType = ButtonSoundType.Click, ButtonUISoundName = defaultClickSound } );
        }
    }
}
