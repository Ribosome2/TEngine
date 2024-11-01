using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Framework;
using TEngine;
using UnityEngine;
using YooAsset;

namespace PatchCode.UIFramework
{
    public class UIManager : Singleton<UIManager>
    {
        public Dictionary<E_UILayer, Transform> uiRootMap = new Dictionary<E_UILayer, Transform>();
        public Dictionary<Type, UIWindowBase> m_OpenUIs = new Dictionary<Type, UIWindowBase>();
        public HashSet<Type> m_OpeningUI = new HashSet<Type>();

        public void InitUI()
        {
            GameObject uiRoot = GameObject.Find("UICanvas");
            uiRoot.SetActive(true);
            GameObject.DontDestroyOnLoad(uiRoot);
            uiRootMap[E_UILayer.Top] = uiRoot.transform.Find("Top");
            uiRootMap[E_UILayer.Normal] = uiRoot.transform.Find("Normal");
            uiRootMap[E_UILayer.Popup] = uiRoot.transform.Find("Popup");
            uiRootMap[E_UILayer.Scene] = uiRoot.transform.Find("Scene");
            uiRootMap[E_UILayer.MainUI] = uiRoot.transform.Find("MainUI");
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
            // ResourceManager.Instance.LoadAssetASync<GameObject>(prefabPath, (go,assetHandle) =>
            // {
            //     var windowsInstance = CreateWindowInstance<T>(go, uiType,assetHandle);
            //     windowsInstance.OpenWithParam(arg);
            // });
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
            var go =await GameModule.Resource.LoadAssetAsync<GameObject>(prefabPath);
            CreateWindowInstance<T>(go, uiType);
            // GameModule.Resource.LoadAssetAsync<GameObject>(prefabPath, (go,assetHandle) =>
            // {
            //     CreateWindowInstance<T>(go, uiType,assetHandle);
            // });
        }

        private UIWindowBase CreateWindowInstance<T>(GameObject go, Type uiType) where T : UIWindowBase
        {
            var layer = UISetting.GetUILayer<T>();
            var parent = uiRootMap[layer];
            var uiGo = GameObject.Instantiate(go, parent);
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