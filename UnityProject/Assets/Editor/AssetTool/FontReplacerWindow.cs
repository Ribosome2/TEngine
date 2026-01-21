using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.IO;

public class FontReplacerWindow : EditorWindow
{
    private string targetFolderPath = "Assets/";
    private Font targetFont;
    private Vector2 scrollPosition;

    [MenuItem("Tools/Font Replacement Tool")]
    public static void ShowWindow()
    {
        GetWindow<FontReplacerWindow>("Font Replacer");
    }

    void OnGUI()
    {
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Font Replacement Settings", EditorStyles.boldLabel);
        
        // 文件夹路径选择
        EditorGUILayout.BeginHorizontal();
        targetFolderPath = EditorGUILayout.TextField("Target Folder", targetFolderPath);
        if (GUILayout.Button("Browse...", GUILayout.Width(80)))
        {
            string path = EditorUtility.OpenFolderPanel("Select Target Folder", "Assets", "");
            if (!string.IsNullOrEmpty(path))
            {
                targetFolderPath = "Assets" + path.Substring(Application.dataPath.Length);
            }
        }
        EditorGUILayout.EndHorizontal();

        // 字体选择
        targetFont = (Font)EditorGUILayout.ObjectField("Target Font", targetFont, typeof(Font), false);

        GUILayout.Space(20);
        
        if (GUILayout.Button("Start Scan & Replace", GUILayout.Height(30)))
        {
            if (targetFont == null)
            {
                EditorUtility.DisplayDialog("Error", "Please select a target font first!", "OK");
                return;
            }

            if (Directory.Exists(targetFolderPath))
            {
                ProcessPrefabs();
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Invalid folder path!", "OK");
            }
        }
    }

    void ProcessPrefabs()
    {
        // 获取所有Prefab的GUID
        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { targetFolderPath });
        int total = guids.Length;
        int processed = 0;
        int modifiedCount = 0;

        foreach (string guid in guids)
        {
            processed++;
            string path = AssetDatabase.GUIDToAssetPath(guid);
            EditorUtility.DisplayProgressBar("Processing Prefabs", 
                $"Scanning ({processed}/{total}) {Path.GetFileName(path)}", 
                (float)processed / total);

            // 加载Prefab
            GameObject prefab = PrefabUtility.LoadPrefabContents(path);
            bool modified = false;

            // 查找所有Text组件
            Text[] textComponents = prefab.GetComponentsInChildren<Text>(true);
            foreach (Text text in textComponents)
            {
                if (text.font != targetFont)
                {
                    text.font = targetFont;
                    modified = true;
                }
            }

            // 保存修改
            if (modified)
            {
                PrefabUtility.SaveAsPrefabAsset(prefab, path);
                modifiedCount++;
            }

            PrefabUtility.UnloadPrefabContents(prefab);
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Complete", 
            $"Process completed!\nTotal: {total} prefabs\nModified: {modifiedCount} prefabs", 
            "OK");
    }
}