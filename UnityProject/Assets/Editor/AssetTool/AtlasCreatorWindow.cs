using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D; // 新增：添加U2D命名空间以访问SpriteAtlas
using System.IO;
using System.Collections.Generic;
using UnityEditor.U2D;

/// <summary>
/// 后面如果需要自动处理或者优化性能，可以参考一下：SpritePostprocessor这个脚本里面的内容
/// </summary>
public class AtlasCreatorWindow : EditorWindow
{
    private static string directoryPath = "Assets/Textures/Sprites";
    private Vector2 scrollPosition;
    private const string ATLAS_FOLDER="Assets/_SpriteAtlases";
    private const string SpriteRootFolders="Assets/GameAssets/Textures/Atlas";


    [MenuItem("Tools/Atlas Creator")]
    public static void ShowWindow()
    {
        GetWindow<AtlasCreatorWindow>("图集创建工具");
    }

    private void OnGUI()
    {
        GUILayout.Label("图集创建设置", EditorStyles.boldLabel);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("目标精灵目录：");
        directoryPath = EditorGUILayout.TextField(directoryPath);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("图集名称：");

        EditorGUILayout.Space();

        if (GUILayout.Button("创建指定目录图集",GUILayout.Height(30)))
        {
            CreateAtlas(directoryPath);
        }

        if (GUILayout.Button($"创建{SpriteRootFolders}下的所有图集",GUILayout.Height(30)))
        {
            BuildAllAtlas();
        }
    }

    public static void BuildAllAtlas()
    {
        var dirs = Directory.GetDirectories(SpriteRootFolders);
        foreach (var dir in dirs)
        {
            CreateAtlas(dir);
        }
    }

    private static void CreateAtlas(string spriteFolderPath)
    {
        if (!ValidateDirectory(spriteFolderPath))
        {
            return;
        }

        List<Sprite> sprites = FindAllSpritesInDirectory(spriteFolderPath);
        if (sprites.Count == 0)
        {
            EditorUtility.DisplayDialog("提示", "未找到任何Sprite资源！", "确定");
            return;
        }

        // 修正：图集保存路径建议放在项目根目录的Atlases下（避免嵌套过深）
        if (!Directory.Exists(ATLAS_FOLDER))
        {
            Directory.CreateDirectory(ATLAS_FOLDER); // 直接在Assets下创建Atlases
        }
        var atlasName = spriteFolderPath.Replace("/", "_").Replace("\\","_");
        string atlasPath = Path.Combine(ATLAS_FOLDER, $"{atlasName}.asset");
        SpriteAtlas atlas = (SpriteAtlas)Activator.CreateInstance(typeof(SpriteAtlas));
        ConfigureAtlasSettings(atlas);
        AddSpritesToAtlas(atlas, sprites);
        Debug.Log("atlasPath: "+atlasPath);
        AssetDatabase.CreateAsset(atlas, atlasPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log( $"图集已创建：{atlasPath}\n包含 {sprites.Count} 个Sprite");
    }

    private static bool ValidateDirectory(string spriteFolderPath)
    {
        if (!AssetDatabase.IsValidFolder(spriteFolderPath))
        {
            EditorUtility.DisplayDialog("错误", $"无效的目录路径：\n{directoryPath}\n请使用项目Assets目录下的有效路径", "确定");
            return false;
        }
        return true;
    }

    private static List<Sprite> FindAllSpritesInDirectory(string dir)
    {
        List<Sprite> sprites = new List<Sprite>();
        string[] spriteGUIDs = AssetDatabase.FindAssets("t:Sprite", new[] { dir });

        foreach (string guid in spriteGUIDs)
        {
            string spritePath = AssetDatabase.GUIDToAssetPath(guid);
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
            if (sprite != null)
            {
                sprites.Add(sprite);
            }
        }
        return sprites;
    }

    private static void ConfigureAtlasSettings(SpriteAtlas atlas)
    {
        atlas.SetIncludeInBuild(true);
        var atlasSetting = new SpriteAtlasTextureSettings();
        atlasSetting.generateMipMaps = false;
        atlasSetting.filterMode = FilterMode.Bilinear;
        atlasSetting.sRGB = true;
        atlas.SetTextureSettings(atlasSetting);

        atlas.SetPlatformSettings(new TextureImporterPlatformSettings()
        {
            compressionQuality =100,
            maxTextureSize = 2048,
            format = TextureImporterFormat.RGBA32,
        });
        atlas.SetPackingSettings(new SpriteAtlasPackingSettings()
        {
            enableRotation = true,
        });

    }

    private static void AddSpritesToAtlas(SpriteAtlas atlas, List<Sprite> sprites)
    {
        atlas.Add(sprites.ToArray());
    }
}
