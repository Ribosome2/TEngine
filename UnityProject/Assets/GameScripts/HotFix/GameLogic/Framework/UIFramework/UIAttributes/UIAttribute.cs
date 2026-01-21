using System;

namespace GameLogic
{
    
    
    [AttributeUsage(AttributeTargets.Class)]
    public class UIAttribute : Attribute
    {
        public string PrefabPath { get; } = string.Empty;
        public E_UILayer Layer { get; } = E_UILayer.Normal;
        public E_UIBackgroundType BackgroundType { get; } = E_UIBackgroundType.None;
        public bool SwitchSceneClose = true;  //切场景的时候是否自动关闭

        public UIAttribute(string prefabPath, E_UILayer layer = E_UILayer.Normal,bool switchSceneClose=true,
            E_UIBackgroundType  backgroundType= E_UIBackgroundType.None)
        {
            PrefabPath = prefabPath.EndsWith(".prefab") ? prefabPath : prefabPath + ".prefab";
            Layer = layer;
            SwitchSceneClose = switchSceneClose;
            BackgroundType = backgroundType;
        }
    }
}