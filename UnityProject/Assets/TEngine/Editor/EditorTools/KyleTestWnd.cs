using System;
using System.Collections;
using System.Threading.Tasks;
using GameLogic;
using UnityEditor;
using UnityEngine;

namespace TEngine
{
    public class KyleTestWnd:EditorWindow
    {
        [MenuItem("KyleKit/KyleTestWnd")]
        static void OpenEntryWnd()
        {
            GetWindow<KyleTestWnd>();
        }

        private void OnGUI()
        {
            if (GUILayout.Button("清空对象池",GUILayout.Height(40)))
            {
                ClearCo();
            }
        }

        async void ClearCo()
        {
            //不延迟好像没有用
            Debug.Log("ClearPool");
            // PoolManager.Instance.Clear();
            await Task.Delay(300); // 转换为毫秒
            Debug.Log("ObjectPoolRelease");
            GameModule.ObjectPool.Release();
            await Task.Delay(300); // 转换为毫秒
            Debug.Log("ObjectPoolReleaseAllUnused");
            GameModule.ObjectPool.ReleaseAllUnused();
        }
    }
}