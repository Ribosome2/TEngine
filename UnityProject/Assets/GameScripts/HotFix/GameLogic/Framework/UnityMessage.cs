using UnityEngine;
using System;
using Framework.Utilities;
using UnityEngine.SceneManagement;

namespace GameLogic
{
    public enum MessageEnum
    {
        Awake = 0,
        OnEnable,
        Start,
        Update,
        LateUpdate,
        FixedUpdate,
        OnDisable,
        OnDestroy,
    }
    
    public class UnityMessage : MonoBehaviourSingleton<UnityMessage>
    {
        /// <summary>
        /// MonoBehaviour 生命周期
        /// </summary>
        public Action<int, int> OnGameObjectLifeCall;
        /// <summary>
        /// MonoDestroy 生命周期
        /// </summary>
        public Action<int> OnGameObjectDestroy;

        /// <summary>
        /// 场景切换成功
        /// </summary>
        public Action<Scene, LoadSceneMode> OnSceneLoaded;

        /// <summary>
        /// 切换活动场景
        /// </summary>
        public Action<Scene, Scene> OnActiveSceneChange;

        /// <summary>
        /// 场景卸载
        /// </summary>
        public Action<Scene> OnSceneUnload;

        /// <summary>
        /// Update 消息
        /// </summary>
        public Action<float, float> OnUpdateMessage;

        /// <summary>
        /// LateUpdate 消息
        /// </summary>
        public Action OnLateUpdateMessage;

        /// <summary>
        /// FixedUpdate 消息
        /// </summary>
        public Action<float> OnFixedUpdateMessage;

        public Action<bool> OnGameFocus;

        public Action<bool> OnGamePause;

        public Action OnGameQuit;
        public Action OnGameDrawGizmos;

        // Start is called before the first frame update
        private void Start()
        {
            SceneManager.sceneLoaded += (scene, mode) =>
            {
                OnSceneLoaded?.Invoke(scene, mode);
            };
            SceneManager.activeSceneChanged += (old, scene) =>
            {
                OnActiveSceneChange?.Invoke(old, scene);
            };
            SceneManager.sceneUnloaded += (scene) =>
            {
                OnSceneUnload?.Invoke(scene);
            };
        }

        // Update is called once per frame
        private void Update()
        {
            OnUpdateMessage?.Invoke(Time.deltaTime, Time.unscaledDeltaTime);
        }

        private void LateUpdate()
        {
            OnLateUpdateMessage?.Invoke();
        }

        private void FixedUpdate()
        {
            OnFixedUpdateMessage?.Invoke(Time.fixedDeltaTime);
        }

        private void OnApplicationFocus(bool focusStatus)
        {
            OnGameFocus?.Invoke(focusStatus);
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            OnGamePause?.Invoke(pauseStatus);
        }

        private void OnApplicationQuit()
        {
            OnGameQuit?.Invoke();
        }

        private void OnDrawGizmos()
        {
            OnGameDrawGizmos?.Invoke();
        }
    }
}
