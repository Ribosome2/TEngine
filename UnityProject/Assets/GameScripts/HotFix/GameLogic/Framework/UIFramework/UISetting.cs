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
            GetUIAttribute<T>(type);
            return s_uiPrefabPathMap[type].PrefabPath;
        }
        
 
        
        public static E_UILayer GetUILayer<T>() where T:UIViewBase
        {
            var type = typeof(T);
            GetUIAttribute<T>(type);
            return s_uiPrefabPathMap[type].Layer;
        }
        

        private static void GetUIAttribute<T>(Type type) where T : UIViewBase
        {
            if (!s_uiPrefabPathMap.TryGetValue(type, out UIAttribute uiAttribute))
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