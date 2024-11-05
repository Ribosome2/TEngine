using UnityEngine;

namespace GameLogic
{
    [SceneHandler(GameSceneTypes.MainCity)]
    public class MainCitySceneHandler:SceneHandlerBase
    {
        public override void OnSceneLoaded()
        {
            base.OnSceneLoaded();
            UIManager.Instance.OpenUI<UIMainCity>();
            Debug.Log("MainCity Scene Loaded--- ");
            
        }

        public override void OnSceneUnloaded()
        {
            base.OnSceneUnloaded();
            Debug.Log("MainCitySceneUnloaded ");
        }
    }
}