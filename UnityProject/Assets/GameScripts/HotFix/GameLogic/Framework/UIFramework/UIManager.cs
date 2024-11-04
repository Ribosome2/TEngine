using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Framework;
using GameBase;
using TEngine;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;

namespace GameLogic
{
    public class UIManager : Singleton<UIManager>
    {
        private Dictionary<E_UILayer, Transform> uiRootMap = new Dictionary<E_UILayer, Transform>();
        private Dictionary<Type, UIWindowBase> m_OpenUIs = new Dictionary<Type, UIWindowBase>();
        private HashSet<Type> m_OpeningUI = new HashSet<Type>();

        public UIManager()
        {
            GameObject uiRoot = GameObject.Find("UICanvas");
            uiRoot.SetActive(true);
            GameObject.DontDestroyOnLoad(uiRoot);
            uiRootMap[E_UILayer.Top] = uiRoot.transform.Find("Top");
            uiRootMap[E_UILayer.Normal] = uiRoot.transform.Find("Normal");
            uiRootMap[E_UILayer.Popup] = uiRoot.transform.Find("Popup");
            uiRootMap[E_UILayer.Scene] = uiRoot.transform.Find("Scene");
            uiRootMap[E_UILayer.MainUI] = uiRoot.transform.Find("MainUI");
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            CloseUIOnSwitchScene();
        }

        public async Task OpenUI<T, TA>(TA arg) where T : UIWindowBase
        {
            var uiType = typeof(T);
            if (m_OpeningUI.Contains(uiType) || m_OpenUIs.ContainsKey(typeof(T)))
            {
                return;
            }

            var prefabPath = UISetting.GetUIPath<T>();
            m_OpeningUI.Add(uiType);
            var go =await GameModule.Resource.LoadAssetAsync<GameObject>(prefabPath);
            CreateWindowInstance<T>(go, uiType);
            var windowsInstance = CreateWindowInstance<T>(go, uiType);
            windowsInstance.OpenWithParam(arg);
        }
        
        public async Task OpenUI<T>() where T : UIWindowBase
        {
            var uiType = typeof(T);
            if (m_OpeningUI.Contains(uiType) || m_OpenUIs.ContainsKey(typeof(T)))
            {
                return;
            }

            var prefabPath = UISetting.GetUIPath<T>();
            m_OpeningUI.Add(uiType);
            var layer = UISetting.GetUILayer<T>();
            var parent = uiRootMap[layer];
            var go =await GameModule.Resource.LoadGameObjectAsync(prefabPath,default,"",parent);
            CreateWindowInstance<T>(go, uiType);

        }

        private UIWindowBase CreateWindowInstance<T>(GameObject uiGo, Type uiType) where T : UIWindowBase
        {
            var windowsInstance = Activator.CreateInstance<T>();
            windowsInstance.InitView(uiGo);
            m_OpenUIs.Add(uiType, windowsInstance);
            m_OpeningUI.Remove(uiType);
            return windowsInstance;
        }

        public void CloseWindow(UIWindowBase windowBase)
        {
            if (m_OpenUIs.ContainsKey(windowBase.GetType()))
            {
                m_OpenUIs.Remove(windowBase.GetType());
                windowBase.OnClose();
                windowBase.Dispose();
                GameObject.Destroy(windowBase.ViewGo);
            }
        }

        public void CloseUIOnSwitchScene()
        {
            Queue<UIWindowBase> deleteQueue = null;
            foreach (var kv in m_OpenUIs)
            {
                if (kv.Value.NeedCloseOnSwitchScene())
                {
                    if (deleteQueue == null)
                    {
                        deleteQueue = new Queue<UIWindowBase>();
                    }

                    deleteQueue.Enqueue(kv.Value);
                }
            }

            if (deleteQueue != null)
            {
                while (deleteQueue.Count > 0)
                {
                    var window = deleteQueue.Dequeue();
                    CloseWindow(window);
                }
            }
        }
    }
}