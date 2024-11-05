using GameLogic;
using UnityEngine;

namespace GameLogic
{
    [SceneHandler(GameSceneTypes.ZombieGame)]
    public class ZombieGameSceneHandler:SceneHandlerBase
    {
        public override void OnSceneLoaded()
        {
            base.OnSceneLoaded();
            UIManager.Instance.OpenUI<PnlZombieGame>();
            UnityMessage.Instance.OnUpdateMessage += OnUpdate;
            Debug.Log("Zombie Game  Scene Loaded--- ");
            ZombieBattleMgr.Instance.Start();
            
        }

        private void OnUpdate(float deltaTime, float unscaleDeltaTime)
        {
            ZombieBattleMgr.Instance.Update();
        }

        public override void OnSceneUnloaded()
        {
            base.OnSceneUnloaded();
            UnityMessage.Instance.OnUpdateMessage -= OnUpdate;
            ZombieBattleMgr.Instance.Dispose();
            Debug.Log("Zombie SceneUnloaded ");
        }
    }
}