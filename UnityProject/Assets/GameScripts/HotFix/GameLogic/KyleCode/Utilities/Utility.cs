using System.Collections.Generic;
using System;
using System.Text;
using System.IO;
using System.Threading;
using System.Security.Cryptography;
using Debug = UnityEngine.Debug;
using System.Reflection;
using System.Text.RegularExpressions;

using Framework;

namespace Framework.Utilities
{
    public class Utility
    {
        private static readonly DateTime m_identity = new (1970, 1, 1, 0, 0, 0, 0);

        public static DateTime Identity
        {
            get { return m_identity; }
        }

        public static double TimeStamp
        {
            get
            {
                TimeSpan ts = DateTime.UtcNow - Identity;
                return ts.TotalMilliseconds;
            }
        }

        private static int guid = 1;

        public static int GUID
        {
            get { return guid++; }
        }

        static Utility()
        {
            _ = MainContext;
        }

        public static Type GetType(string className, out Assembly loadedAssembly, string assemblyName = "")
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            loadedAssembly = null;
            foreach (var assembly in assemblies)
            {
                if (string.IsNullOrEmpty(assemblyName) || assembly.FullName.Contains(assemblyName))
                {
                    loadedAssembly = assembly;
                    foreach (var type in assembly.GetTypes())
                        if (type.Name == className || type.FullName == className)
                            return type;
                }
            }
            #if UNITY_EDITOR
            Debug.LogWarning($"not fount class name:{className}");
            #endif
            return null;
        }
        

        
        public static string String2MD5(string content)
        {
            MD5 md5 = MD5.Create();
            byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(content));
            StringBuilder sb = new StringBuilder();
            for (int index = 0; index < data.Length; index++)
                sb.Append(data[index].ToString("x2"));

            return sb.ToString();
        }

        public static string File2MD5(string filePath)
        {
            string filemd5;
            using (var fileStream = File.OpenRead(filePath))
            {
                MD5 md5 = MD5.Create();
                byte[] data = md5.ComputeHash(fileStream);
                filemd5 = BitConverter.ToString(data).Replace("-", "").ToLower();
            }

            return filemd5;
        }

        public static string File2MD5(IEnumerable<string> files)
        {
            StringBuilder md5Str = new StringBuilder();
            var list = new List<string>(files);
            list.Sort();
            files = list.ToArray();
            foreach (string path in files)
                md5Str.Append(File2MD5(path));

            return String2MD5(md5Str.ToString());
        }

        
        private static SynchronizationContext mainContext;
        public static SynchronizationContext MainContext
        {
            get{return mainContext ??= SynchronizationContext.Current ?? new SynchronizationContext();}
        }
        
        public static string ConvertStackTrace(string stackTrace)
        {
            stackTrace = Regex.Replace(stackTrace, @"in\b([\s\S]*?):(\d+)\b", (m) =>
            {
                string path = PathUtility.GetProjectRelativePath(m.Groups[1].Value).Trim();
                string line = m.Groups[2].Value;
                return $"in <a href=\"{path}\" line=\"{line}\">{path}:{line}</a>";
            });
            return stackTrace;
        }
    }
}