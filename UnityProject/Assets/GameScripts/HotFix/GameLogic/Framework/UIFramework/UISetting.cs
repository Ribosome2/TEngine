using System;
using System.Collections.Generic;

namespace GameLogic
{
    public class UISetting
    {
        private static Dictionary<Type, UIAttribute> s_uiPrefabPathMap = new Dictionary<Type, UIAttribute>();
        public static string GetUIPath<T>() where T:UIViewBase
        {
            var type = typeof(T);
            CheckInitUIAttribute(type);
            return s_uiPrefabPathMap[type].PrefabPath;
        }
        
 
        
        public static E_UILayer GetUILayer<T>() where T:UIViewBase
        {
            var type = typeof(T);
            CheckInitUIAttribute(type);
            return s_uiPrefabPathMap[type].Layer;
        }
        
        public static E_UIBackgroundType GetUIBackgroundType<T>() where T:UIViewBase
        {
            var type = typeof(T);
            CheckInitUIAttribute(type);
            return s_uiPrefabPathMap[type].BackgroundType;
        }
        
        public static bool GetIsUnloadOnSwitchScene<T>() where T:UIViewBase
        {
            var type = typeof(T);
            CheckInitUIAttribute(type);
            return s_uiPrefabPathMap[type].SwitchSceneClose;
        }

        public static bool GetIsUnloadOnSwitchScene(Type type)
        {
            CheckInitUIAttribute(type);
            return s_uiPrefabPathMap[type].SwitchSceneClose;
        }

        private static void CheckInitUIAttribute(Type type)
        {
            if (!s_uiPrefabPathMap.ContainsKey(type))
            {
                var attribute = type.GetCustomAttributes(typeof(UIAttribute), false);
                if (attribute.Length > 0)
                {
                    s_uiPrefabPathMap.Add(type, (attribute[0] as UIAttribute));
                }
            }
        }
    }
}