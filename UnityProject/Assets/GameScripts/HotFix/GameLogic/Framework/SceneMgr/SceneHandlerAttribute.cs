using System;

namespace GameLogic
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SceneHandlerAttribute:Attribute
    {
        public string SceneName { get; } = string.Empty;

        public SceneHandlerAttribute(string sceneName)
        {
            SceneName = sceneName;
        }
    }
}