using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using YooAsset;
using YooAsset.Editor;
using BuildResult = UnityEditor.Build.Reporting.BuildResult;

namespace TEngine.Editor
{
    /// <summary>
    /// 打包工具类。
    /// <remarks>通过CommandLineReader可以不前台开启Unity实现静默打包以及CLI工作流，详见CommandLineReader.cs example1</remarks>
    /// </summary>
    public class ReleaseToolsWindow:EditorWindow
    {
        public static EFileNameStyle bundleNameStyle=EFileNameStyle.BundleName;
        public static BuildOptions buildOption=BuildOptions.Development;
        public static bool buildAB=true;
        public static bool buildAtlas=true;
        [MenuItem("KyleKit/打包窗口")]
        public static void OpenWind()
        {
            GetWindow<ReleaseToolsWindow>();
        }
        private void OnGUI()
        {
            bundleNameStyle = (EFileNameStyle)EditorGUILayout.EnumFlagsField("ab命名风格：",bundleNameStyle);
            buildOption = (BuildOptions)EditorGUILayout.EnumFlagsField("BuildOption：",buildOption);
            buildAB = EditorGUILayout.Toggle("BuildAB：",buildAB);
            buildAtlas = EditorGUILayout.Toggle("buildAtlas：",buildAtlas);
            using (new GUILayout.HorizontalScope())
            {
                if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
                {
                    if (GUILayout.Button("打包apk", GUILayout.Height(50)))
                    {
                        CheckBuildAtlas();
                        AutomationBuildAndroidLocalMode();
                    }
                }
                else if(EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows || EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows64)
                {

                    if (GUILayout.Button("打包PC", GUILayout.Height(50)))
                    {
                        CheckBuildAtlas();
                        AutomationBuildPCLocalMode();
                    }
                }
            }
        }


        void CheckBuildAtlas()
        {
            if (buildAtlas)
            {
                AtlasCreatorWindow.BuildAllAtlas();
            }
        }


        private static YooAsset.Editor.BuildResult BuildInternal(BuildTarget buildTarget, string outputRoot, string packageVersion = "1.0",
            EBuildPipeline buildPipeline = EBuildPipeline.ScriptableBuildPipeline)
        {
            Debug.Log($"开始构建 : {buildTarget}");

            IBuildPipeline pipeline = null;
            BuildParameters buildParameters = null;

            if (buildPipeline == EBuildPipeline.BuiltinBuildPipeline)
            {
                // 构建参数
                BuiltinBuildParameters builtinBuildParameters = new BuiltinBuildParameters();

                // 执行构建
                pipeline = new BuiltinBuildPipeline();
                buildParameters = builtinBuildParameters;

                builtinBuildParameters.CompressOption = ECompressOption.LZ4;
            }
            else
            {
                ScriptableBuildParameters scriptableBuildParameters = new ScriptableBuildParameters();

                // 执行构建
                pipeline = new ScriptableBuildPipeline();
                buildParameters = scriptableBuildParameters;

                scriptableBuildParameters.CompressOption = ECompressOption.LZ4;
            }

            buildParameters.BuildOutputRoot = AssetBundleBuilderHelper.GetDefaultBuildOutputRoot();
            Debug.Log("BuildOutputRoot:"+buildParameters.BuildOutputRoot);
            buildParameters.BuildinFileRoot = AssetBundleBuilderHelper.GetStreamingAssetsRoot();
            buildParameters.BuildPipeline = buildPipeline.ToString();
            buildParameters.BuildTarget = buildTarget;
            buildParameters.BuildMode = EBuildMode.IncrementalBuild;
            buildParameters.PackageName = "DefaultPackage";
            buildParameters.PackageVersion = packageVersion;
            buildParameters.VerifyBuildingResult = true;
            buildParameters.FileNameStyle = bundleNameStyle;


            buildParameters.BuildinFileCopyOption = EBuildinFileCopyOption.ClearAndCopyAll;
            buildParameters.BuildinFileCopyParams = string.Empty;
            buildParameters.EncryptionServices = null;


            // 启用共享资源打包
            buildParameters.EnableSharePackRule = true;

            var buildResult = pipeline.Run(buildParameters, true);
            if (buildResult.Success)
            {

                Debug.Log($"构建成功 : {buildResult.OutputPackageDirectory}");
            }
            else
            {
                Debug.LogError($"构建失败 : {buildResult.ErrorInfo}");
            }

            return buildResult;

        }

        private static void CopyBundlesToFolder(string bundleFolder, string targetFolder)
        {
            if (string.IsNullOrEmpty(bundleFolder) == false)
            {
                Debug.Log("找到Bundle目录:"+bundleFolder);

                var allFiles = Directory.GetFiles(bundleFolder);
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



        // 构建版本相关
        private static string GetBuildPackageVersion()
        {
            int totalMinutes = DateTime.Now.Hour * 60 + DateTime.Now.Minute;
            return DateTime.Now.ToString("yyyy-MM-dd") + "-" + totalMinutes;
        }

        public static void AutomationBuildAndroidLocalMode()
        {
            BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
            BuildDLLCommand.BuildAndCopyDlls(target);
            AssetDatabase.Refresh();
            if (buildAB)
            {
                var buildResult=BuildInternal(target, outputRoot: Application.dataPath + "/../Bundles", packageVersion: GetBuildPackageVersion());
                if (!buildResult.Success)
                {
                    return;
                }
                Debug.Log("AB Directory :"+buildResult.OutputPackageDirectory);
                CopyBundlesToFolder(buildResult.OutputPackageDirectory,
                    AssetBundleBuilderHelper.GetStreamingAssetsRoot()+"DefaultPackage/");
                AssetDatabase.Refresh();
            }
            AssetDatabase.Refresh();
            var postFix = (buildOption & BuildOptions.Development) != 0 ? "dev" : "release";
            BuildImp(BuildTargetGroup.Android, BuildTarget.Android, $"{Application.dataPath}/../Build/Android/{GetBuildPackageVersion()}Android_{postFix}.apk");
        }

        public static void AutomationBuildPCLocalMode()
        {
            BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
            BuildDLLCommand.BuildAndCopyDlls(target);
            AssetDatabase.Refresh();
            if (buildAB)
            {
                var buildResult=BuildInternal(target, outputRoot: Application.dataPath + "/../Bundles", packageVersion: GetBuildPackageVersion());
                if (!buildResult.Success)
                {
                    return;
                }
                Debug.Log("AB Directory :"+buildResult.OutputPackageDirectory);
                CopyBundlesToFolder(buildResult.OutputPackageDirectory,
                    AssetBundleBuilderHelper.GetStreamingAssetsRoot()+"DefaultPackage/");
                AssetDatabase.Refresh();
            }
            AssetDatabase.Refresh();
            BuildImp(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64, $"{Application.dataPath}/../Build/PC/BlockDemo_PC.exe");
        }



        public static void BuildImp(BuildTargetGroup buildTargetGroup, BuildTarget buildTarget, string locationPathName)
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(buildTargetGroup, buildTarget);
            AssetDatabase.Refresh();

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = EditorBuildSettings.scenes.Select(scene => scene.path).ToArray(),
                locationPathName = locationPathName,
                targetGroup = buildTargetGroup,
                target = buildTarget,
                options =buildOption,
            };
            var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            BuildSummary summary = report.summary;
            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log($"Build success: {summary.totalSize / 1024 / 1024} MB");
            }
            else
            {
                Debug.Log($"Build Failed" + summary.result);
            }
        }
    }
}
