using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
        if (GUILayout.Button("CopyBundlesToStreaming",GUILayout.Height(50)))
        {
            var targetFolder = "Assets/StreamingAssets/package/DefaultPackage";
            ClearFolder(targetFolder);
            CopyBundlesToFolder(bundleFolder, targetFolder);
        }
        GUILayout.EndHorizontal();
        
        if (GUILayout.Button("复制热更代码到Bundle目录",GUILayout.Height(50)))
        {
            CopyHotUpdateCodes();
        }
        
        GUILayout.Space(20);
        if (GUILayout.Button("运行Exe",GUILayout.Height(30)))
        {
            RunExe();
        }
    }

    private static void CopyBundlesToFolder(string bundleFolder, string targetFolder)
    {
        var dirs = Directory.GetDirectories(bundleFolder);
        // 使用 LINQ 对目录路径按字母顺序降序排序(版本号大的会在前面）
        var sortedDirs = dirs.OrderByDescending(dir => dir);
        bool found=false;
        foreach (string dir in sortedDirs)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(dir);
            //bundle生成目录格式：“2024-06-18-1003”
            if (Regex.Match(directoryInfo.Name, "[\\d_]+").Success)
            {
                found = true;
                // CopyHotUpdateCodes();
                Debug.Log("找到生成目录"+directoryInfo.FullName);
                var allFiles = Directory.GetFiles(dir);
                int count=0;
                foreach (string filePath in allFiles)
                {
                    var fileInfo = new FileInfo(filePath);
                    var newPath = Path.Combine(targetFolder, fileInfo.Name);
                    File.Copy(filePath,newPath,true);
                    count++;
                }
                Debug.Log($"复制了 {count} 个文件到 {targetFolder}");
                break;
            }
        }

        if (found == false)
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

    private static void RunExe()
    {
        try
        {
            // 定义可执行文件的路径
            string exePath = @"Builds\Windows\Release_Windows.exe";

            // 启动进程
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

    static void CopyMetaDll(string buildTargetName,string dllName)
    {
        File.Copy($"HybridCLRData/AssembliesPostIl2CppStrip/{buildTargetName}/{dllName}.dll",$"Assets/PatchGameRes/Code/{dllName}.dll.bytes",true);
    }
}
