using System;

namespace GameLogic
{
    public class ECSHelper
    {
        public static T AddComponent<T>(GameUnit unit) where T:GameComponent
        {
            var instance = Activator.CreateInstance<T>();
            return instance;
        }
    }
}