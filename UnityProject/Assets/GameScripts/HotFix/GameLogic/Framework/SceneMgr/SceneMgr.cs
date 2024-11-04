using System;
using System.Collections.Generic;
using System.Reflection;
using Framework;
using GameBase;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameLogic
{
    public class SceneMgr:Singleton<SceneMgr>
    {
        private static Dictionary<string, Type> m_sceneHandlerMap;
        private Scene m_currentScene;
        private  static SceneHandlerBase m_currentSceneHandler;
        
        
        // #region 公开接口
        // public void GoToScene(string sceneLocation,Action completeCallback=null)
        // {
        //     ResourceManager.Instance.LoadSceneAsync(sceneLocation,completeCallback);
        // }
        // public Scene GetCurrentScene()
        // {
        //     return m_currentScene;
        // }
        // #endregion
        
        public static void Init()
        {
            InitHandlerMap();
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.activeSceneChanged += OnActiveSceneChange;
            SceneManager.sceneUnloaded += OnSceneUnload;
        }
        private static void InitHandlerMap()
        {
            m_sceneHandlerMap = new Dictionary<string, Type>();
#if UNITY_EDITOR
            Dictionary<string,Type> duplicateCheckMap = new Dictionary<string, Type>();
#endif
            // this is making the assumption that all assemblies we need are already loaded.
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    var attribs = type.GetCustomAttributes(typeof(SceneHandlerAttribute), false);
                    if (attribs != null && attribs.Length > 0)
                    {
                        var attribute = (SceneHandlerAttribute)attribs[0];
#if UNITY_EDITOR
                        if (duplicateCheckMap.ContainsKey(attribute.SceneName))
                        {
                            Debug.LogError($"SceneName {attribute.SceneName} is Duplicated : {duplicateCheckMap[attribute.SceneName]} and {type}  ");
                            continue;
                        }
                        duplicateCheckMap.Add(attribute.SceneName,type);
#endif
                        m_sceneHandlerMap.Add(attribute.SceneName,type);
                    }
                }
            }
        }

        private static void OnSceneUnload(Scene scene)
        {
            Debug.Log("SceneUnloaded "+scene.name+" scenePath:"+scene.path);
            if (m_currentSceneHandler != null)
            {
                m_currentSceneHandler.OnSceneUnloaded();
                m_currentSceneHandler = null;
            }
        }

        private static void OnActiveSceneChange(Scene oldScene, Scene newScene)
        {
            Debug.Log("OnActiveSceneChange "+oldScene.name+" newScenePath:"+newScene);
        }

        private static void OnSceneLoaded(Scene sceneLoaded, LoadSceneMode loadSceneMode)
        {
            Debug.Log("OnSceneLoaded "+sceneLoaded.name+" loadSceneMode:"+loadSceneMode);
            if (loadSceneMode == LoadSceneMode.Single)
            {
                Type handlerType;
                if (!m_sceneHandlerMap.TryGetValue(sceneLoaded.path, out  handlerType))
                {
                    handlerType = typeof(SceneHandlerBase);
                    Debug.LogError($"Null SceneHandlerFor: {sceneLoaded.path},use default scene handler  ");
                }
                m_currentSceneHandler=(SceneHandlerBase)Activator.CreateInstance(handlerType);
                m_currentSceneHandler.OnSceneLoaded();
            }
        }


        
    }
}