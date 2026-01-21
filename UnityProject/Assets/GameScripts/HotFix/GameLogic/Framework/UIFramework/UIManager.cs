using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TEngine;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameLogic
{
    public class UIManager : GameBase.Singleton<UIManager>
    {
        private Dictionary<E_UILayer, RectTransform> uiRootMap = new();
        private Dictionary<Type, UIWindowBase> m_OpenUIs = new();
        private HashSet<Type> m_OpeningUI = new();
        private Camera uiCamera;
        private BlurScreenManager _blurScreenManager=new ();

        public BlurScreenManager BlurScreenManager => _blurScreenManager;

        public Dictionary<Type, UIWindowBase> GetUIMapForEditor()
        {
            return m_OpenUIs;
        }

        public Camera UICamera
        {
            get
            {
                return uiCamera;
            }
        }
        public UIManager()
        {
            var starSceneBgUI=GameObject.Find("StartSceneBgUI");
            if (starSceneBgUI)
            {
                GameObject.Destroy(starSceneBgUI);
            }

            GameObject uiRoot = GameObject.Find("UICanvas");
            if (uiRoot.GetComponent<Canvas>().renderMode == RenderMode.ScreenSpaceCamera)
            {
                uiCamera = GameObject.Find("UIRoot/UICamera").GetComponent<Camera>();
                uiRoot.SetActive(true);
            }

            GameObject.DontDestroyOnLoad(uiRoot);
            var rootTrans = uiRoot.transform;
            uiRootMap[E_UILayer.Top] = rootTrans.Find("Top").GetComponent<RectTransform>();
            uiRootMap[E_UILayer.Normal] = rootTrans.Find("Normal").GetComponent<RectTransform>();
            uiRootMap[E_UILayer.Popup] = rootTrans.Find("Popup").GetComponent<RectTransform>();
            uiRootMap[E_UILayer.Scene] = rootTrans.Find("Scene").GetComponent<RectTransform>();
            uiRootMap[E_UILayer.MainUI] = rootTrans.Find("MainUI").GetComponent<RectTransform>();
            uiRootMap[E_UILayer.Guide] = rootTrans.Find("Guide").GetComponent<RectTransform>();
            uiRootMap[E_UILayer.TopMost] = rootTrans.Find("TopMost").GetComponent<RectTransform>();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public RectTransform GetUILayerRoot(E_UILayer layer)
        {
            if (uiRootMap.TryGetValue(layer, out var tr))
            {
                return tr;
            }
            return null;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            CloseUIOnSwitchScene();
        }


        private GameObject debugGO;
        public void SetupCameraStack()
        {
            var mainCamera = GameObject.FindWithTag("MainCamera");
            // Debug.LogError("--------SetCameraStack"+mainCamera);
            // if (mainCamera && uiCamera)
            // {
            //     var cameraData = mainCamera.GetComponent<Camera>().GetUniversalAdditionalCameraData();
            //     cameraData.cameraStack.Add(uiCamera);
            //     // Debug.LogError("SetCameraStack--- uiCamera"+uiCamera);
            // }
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
            var layer = UISetting.GetUILayer<T>();
            var parent = uiRootMap[layer];
            var go =await GameModule.Resource.LoadGameObjectAsync(prefabPath,parent,default,"");
            var windowsInstance = CreateWindowInstance<T>(go, uiType);
            windowsInstance.OpenWithParam(arg);

            EventCenter.Fire(GlobalEvent.ON_UI_OPEN, uiType.ToString());
        }

        public async Task OpenUI<T>() where T : UIWindowBase
        {
            var uiType = typeof(T);
            // Debug.LogError("Open " +uiType);
            if (m_OpeningUI.Contains(uiType) || m_OpenUIs.ContainsKey(typeof(T)))
            {
                return;
            }

            var prefabPath = UISetting.GetUIPath<T>();
            m_OpeningUI.Add(uiType);
            var layer = UISetting.GetUILayer<T>();
            var backgroundType = UISetting.GetUIBackgroundType<T>();
            var parent = uiRootMap[layer];
            var go =await GameModule.Resource.LoadGameObjectAsync(prefabPath,parent,default,"");
            if (backgroundType == E_UIBackgroundType.BlurMask)
            {
                _blurScreenManager.DoCacheTexture(() =>
                {
                    CreateWindowInstance<T>(go, uiType);
                    EventCenter.Fire(GlobalEvent.ON_UI_OPEN, uiType.ToString());
                },uiType.Name);
            }
            else
            {
                CreateWindowInstance<T>(go, uiType);
                EventCenter.Fire(GlobalEvent.ON_UI_OPEN, uiType.ToString());
            }

        }

        public T GetUIByType<T>() where T:UIWindowBase
        {
            m_OpenUIs.TryGetValue(typeof(T), out var uiWindowBase);
            return uiWindowBase as T;
        }


        public UIWindowBase GetUI(string uiTypeStr)
        {
            Type uiType = Type.GetType(uiTypeStr);
            m_OpenUIs.TryGetValue(uiType, out var uiWindowBase);
            return uiWindowBase;
        }


        public void CloseUI<T>() where T:UIWindowBase
        {
            m_OpenUIs.TryGetValue(typeof(T), out var uiWindowBase);
            if (uiWindowBase != null)
            {
                CloseWindow(uiWindowBase);
            }
        }
        private UIWindowBase CreateWindowInstance<T>(GameObject uiGo, Type uiType) where T : UIWindowBase
        {
            var windowsInstance = Activator.CreateInstance<T>();
            m_OpenUIs.Add(uiType, windowsInstance);
            m_OpeningUI.Remove(uiType);
            var backgroundType = UISetting.GetUIBackgroundType<T>();
            windowsInstance.BackgroundType = backgroundType;
            try
            {
                windowsInstance.InitView(uiGo);
            }
            catch (Exception e)
            {
               Debug.LogError($"Open {windowsInstance} error: {e}");
            }

            return windowsInstance;
        }

        public void CloseWindow(UIWindowBase windowBase)
        {
            Type type = windowBase.GetType();
            if (m_OpenUIs.ContainsKey(type))
            {
                m_OpenUIs.Remove(type);
                windowBase.OnClose();
                windowBase.Dispose();
                GameObject.Destroy(windowBase.ViewGo);

                EventCenter.Fire(GlobalEvent.ON_UI_CLOSE, type.ToString());
            }
        }

        private void CloseUIOnSwitchScene()
        {
            Queue<UIWindowBase> deleteQueue = null;
            foreach (var kv in m_OpenUIs)
            {
                var type = kv.Value.GetType();
                if (UISetting.GetIsUnloadOnSwitchScene(type))
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
