using UnityEngine;
using System.Text.RegularExpressions;
using System.IO;
using System;

namespace Framework.Utilities
{
    public class PathUtility
    {
        private const char m_pathSeparator_win = '\\';
        private const char m_pathSeparator_os = '/';
        
        public static string ConvertPathSeparator(string path)
        {
            return path.IndexOf(m_pathSeparator_win) != -1 
                ? path.Replace(m_pathSeparator_win, m_pathSeparator_os) 
                : path;
        }
        
        private static string m_ProjectPath;
        public static string ProjectPath
        {
            get
            {
                if(string.IsNullOrEmpty(m_ProjectPath))
                    m_ProjectPath = Directory.GetParent(Application.dataPath).FullName;
                return m_ProjectPath;
            }
        }

        private static string platformFolder = string.Empty;
        public static string PlatformFolder
        {
            get
            {
#if UNITY_EDITOR
                string PlatformStr = "Platform";
                UnityEditor.BuildTarget target = UnityEditor.EditorUserBuildSettings.activeBuildTarget;
                switch (target)
                {
                    case UnityEditor.BuildTarget.iOS:
                        PlatformStr = RuntimePlatform.IPhonePlayer.ToString();
                        break;
                    case UnityEditor.BuildTarget.Android:
                        PlatformStr = RuntimePlatform.Android.ToString();
                        break;
                    case UnityEditor.BuildTarget.StandaloneWindows:
                    case UnityEditor.BuildTarget.StandaloneWindows64:
                        PlatformStr = RuntimePlatform.WindowsPlayer.ToString();
                        break;
                    case UnityEditor.BuildTarget.StandaloneOSX:
                        PlatformStr = RuntimePlatform.OSXPlayer.ToString();
                        break;
                    case UnityEditor.BuildTarget.StandaloneLinux64:
                        PlatformStr = RuntimePlatform.LinuxPlayer.ToString();
                        break;
                    case UnityEditor.BuildTarget.WebGL:
                        PlatformStr = RuntimePlatform.WebGLPlayer.ToString();
                        break;
                    default: throw new ArgumentOutOfRangeException();
                }
                return PlatformStr;
#else
                if(string.IsNullOrEmpty(platformFolder))
                    platformFolder = Application.platform.ToString();
                return platformFolder;
#endif
            }
        }

        private static string streamBundleFolder = string.Empty;
        public static string StreamBundleFolder
        {
            get
            {
#if UNITY_EDITOR
                return Path.Combine(Application.streamingAssetsPath, PlatformFolder);
#else
                if(string.IsNullOrEmpty(streamBundleFolder))
                    streamBundleFolder = Path.Combine(Application.streamingAssetsPath, PlatformFolder);
                return streamBundleFolder;
#endif
            }
        }

        private static string localDataPath = string.Empty;
        public static string LocalDataPath
        {
            get
            {
#if UNITY_EDITOR
                return Path.Combine(ProjectPath, "PersistentData");
#else
                if(string.IsNullOrEmpty(localBundleFolder))
                    localBundleFolder = Application.persistentDataPath;
                return localBundleFolder;
#endif
            }
        }

        private static string localBundleFolder = string.Empty;
        public static string LocalBundleFolder
        {
            get
            {
#if UNITY_EDITOR
                return Path.Combine(LocalDataPath, PlatformFolder);
#else
                if(string.IsNullOrEmpty(localBundleFolder))
                    localBundleFolder = Path.Combine(LocalDataPath, PlatformFolder);
                return localBundleFolder;
#endif
            }
        }

        private static string tempDataPath = string.Empty;
        public static string TempDataPath
        {
            get
            {
                #if UNITY_EDITOR
                    return Path.Combine(Application.temporaryCachePath, "GameTemp");
                #else
                    if(string.IsNullOrEmpty(tempDataPath))
                        tempDataPath = Application.temporaryCachePath;
                    return tempDataPath;
                #endif
                
            }
        }

        private static string tempBundleFolder = string.Empty;
        /// <summary>
        /// 用于安卓平台, 缓存包里的Bundle, 方便做解密处理
        /// </summary>
        public static string TempCachedBundleFolder
        {
            get
            {
                #if UNITY_EDITOR
                    return Path.Combine(TempDataPath, PlatformFolder);
                #else
                    if(string.IsNullOrEmpty(tempBundleFolder))
                        tempBundleFolder = Path.Combine(TempDataPath, PlatformFolder);
                    return tempBundleFolder;
                #endif
            }
        }

        /// <summary>
        /// 判断路径是否是文件夹
        /// 注意：路径一定要真是存在的，如果路径不存在会报错
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsDirectory(string path)
        {
            bool isDirectory = false;
            try
            {
                isDirectory = File.GetAttributes(path).CompareTo(FileAttributes.Directory) == 0;
            }
            catch (Exception e)
            {
                Debug.LogError("文件路径不存在！！！\n" + e.Message);
            }
            return isDirectory;
        }

        // 获取文件或目录名字，功能参考python的os.path.basename
        public static string GetBaseName(string path)
        {
            Match ret = Regex.Match(path, @"[/\\]([\w ]+\.?\w*)[/\\]?$");

            return ret.Groups.Count >= 2 ? ret.Groups[1].Value : path;
        }

        public static string GetDirectoryName(string path)
        {
            path = ConvertPathSeparator(path);
            if (path.EndsWith("/")) path = path[..^1];
            return IsDirectory(path) ? path : Path.GetDirectoryName(path);
        }

        public static void MakeSureDirExist(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        public static void MakeSureDirEmpty(string path)
        {
            if(Directory.Exists(path)) Directory.Delete(path, true);
            MakeSureDirExist(path);
        }

        public static string NormalizePath(string path)
        {
            return path.Replace(Path.DirectorySeparatorChar == m_pathSeparator_win 
                ? m_pathSeparator_os 
                : m_pathSeparator_win, Path.DirectorySeparatorChar);
        }
        
        public static string GetProjectRelativePath(string path)
        {
            return path.Replace(m_pathSeparator_win, m_pathSeparator_os)
                       .Replace(ProjectPath, "")[1..];
        }
    }
    
    [AttributeUsage(AttributeTargets.Class)]
    public class FilePathAttribute : Attribute
    {
        internal string filepath;
        public FilePathAttribute(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("Invalid relative path (it is empty)");  
            
            if (path[0] == '/')
                path = path[1..];
            
            filepath = path;
        }
    }
}
