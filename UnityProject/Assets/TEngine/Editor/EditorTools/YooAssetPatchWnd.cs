using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TEngine.Editor;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class YooAssetPatchWnd : EditorWindow
{
    [MenuItem("YooAsset/热更工具窗口")]
    public static void OpenWindow()
    {
        GetWindow<YooAssetPatchWnd>();
    }

    private const string fileServerPathPref = "LocalPatchFileServer";
    private void OnGUI()
    {
        GUILayout.Label("BuildTarget: "+EditorUserBuildSettings.activeBuildTarget.ToString());
        string patchServerFolder = EditorPrefs.GetString(fileServerPathPref);
        GUILayout.BeginHorizontal();
        EditorGUILayout.TextField("local FileServer: ",  patchServerFolder);
        if (GUILayout.Button("Select", GUILayout.ExpandWidth(false)))
        {
            var path = EditorUtility.OpenFolderPanel("select file Folder", patchServerFolder, "");
            if (string.IsNullOrEmpty(path) == false)
            {
                EditorPrefs.SetString(fileServerPathPref, path);
            }
        }

        string bundleFolder = $"Bundles\\{EditorUserBuildSettings.activeBuildTarget}\\DefaultPackage";
        GUILayout.EndHorizontal();
        GUILayout.Space(20);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("ClearCDNFolder",GUILayout.Height(50)))
        {
            ClearFolder(patchServerFolder);
            ShowNotification(new GUIContent("清理完毕"));
        }
        if (GUILayout.Button("CopyBundlesToCDN",GUILayout.Height(50)))
        {
            var targetFolder = patchServerFolder;
            CopyBundlesToFolder(bundleFolder, targetFolder);
        }
        if (GUILayout.Button("复制到HFS目录",GUILayout.Height(50)))
        {
            var targetFolder = @"../Tools\HFS\Default_0\"+SettingsUtils.GetPlatformName();
            EnsureDirectoryExists(targetFolder);
            CopyBundlesToFolder(bundleFolder, targetFolder);
        }
        
        if (GUILayout.Button("CopyBundlesToStreaming",GUILayout.Height(50)))
        {
            var targetFolder = "Assets/StreamingAssets/package/DefaultPackage";
            EnsureDirectoryExists(targetFolder);
            ClearFolder(targetFolder);
            CopyBundlesToFolder(bundleFolder, targetFolder);
        }
        GUILayout.EndHorizontal();
        
        if (GUILayout.Button("复制热更代码到Bundle目录",GUILayout.Height(50)))
        {
            CopyHotUpdateCodes();
        }
        
        GUILayout.Space(20);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("运行exe",GUILayout.Height(30)))
        {
            string exePath = @"Builds\Windows\Release_Windows.exe";
            RunExe(exePath);
        }
        if (GUILayout.Button("运行HFS",GUILayout.Height(30)))
        {
            OpenFolderHelper.Execute("../Tools/HFS");
            // RunExe("../Tools/HFS"); //这样没法运行，就先直接打开hfs所在目录了，点击运行
        }
        GUILayout.EndHorizontal();
    }

    private static void CopyBundlesToFolder(string bundleFolder, string targetFolder)
    {
        var dirs = Directory.GetDirectories(bundleFolder);
        var targetDir = string.Empty;
        long maxNum = -1;
        foreach (string dir in dirs)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(dir);
            //bundle生成目录格式：“2024-06-18-1003”
            if (Regex.Match(directoryInfo.Name, "[\\d_]+").Success)
            {
                Debug.Log("找到生成目录"+directoryInfo.FullName);

                var folderToNum = long.Parse(directoryInfo.Name.Replace("-", ""));
                if (folderToNum > maxNum)
                {
                    maxNum = folderToNum;
                    targetDir = dir;
                }
            }
        }
        

        if (string.IsNullOrEmpty(targetDir) == false)
        {
            Debug.Log("找到Bundle目录:"+targetDir);

            var allFiles = Directory.GetFiles(targetDir);
            int count=0;
            foreach (string filePath in allFiles)
            {
                var fileInfo = new FileInfo(filePath);
                var newPath = Path.Combine(targetFolder, fileInfo.Name);
                File.Copy(filePath,newPath,true);
                count++;
            }
            Debug.Log($"复制了 {count} 个文件到 {targetFolder}");
        }
        else
        {
            Debug.Log("没有找到目标目录： "+ bundleFolder);
        }
    }

    private static void ClearFolder(string targetFolder)
    {
        var allFiles = Directory.GetFiles(targetFolder);
        foreach (string file in allFiles)
        {
            File.Delete(file);
        }
    }

    private static void RunExe(string exePath)
    {
        if (!File.Exists(exePath))
        {
            Debug.LogError($"{exePath} not exist ");
        }
        try
        {
            Debug.Log("try start process :"+ exePath);
            Process.Start(exePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine("处理过程中发生错误: " + ex.Message);
        }
    }

    private static void CopyHotUpdateCodes()
    {
        var buildTargetName = EditorUserBuildSettings.activeBuildTarget.ToString();
        File.Copy($"HybridCLRData/HotUpdateDlls/{buildTargetName}/HotUpdate.dll","Assets/PatchGameRes/Code/HotUpdate.dll.bytes",true);
        //补充元数据
        CopyMetaDll(buildTargetName, "mscorlib");
        CopyMetaDll(buildTargetName, "System");
        CopyMetaDll(buildTargetName, "System.Core");
        AssetDatabase.Refresh();
    }
    public static void EnsureDirectoryExists(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentException("Path cannot be null or empty.", nameof(path));
        }

        // 如果目录不存在，则创建它
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            Console.WriteLine($"Directory '{path}' has been created.");
        }
        else
        {
            Console.WriteLine($"Directory '{path}' already exists.");
        }
    }
    static void CopyMetaDll(string buildTargetName,string dllName)
    {
        File.Copy($"HybridCLRData/AssembliesPostIl2CppStrip/{buildTargetName}/{dllName}.dll",$"Assets/PatchGameRes/Code/{dllName}.dll.bytes",true);
    }
}
