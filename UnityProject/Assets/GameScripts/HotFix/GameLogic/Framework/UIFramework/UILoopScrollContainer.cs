// using System;
// using System.Collections.Generic;
// using Framework;
// using UnityEngine;
// using UnityEngine.UI;
// using YooAsset;
//
// namespace GameLogic
// {
//     public class UILoopScrollContainer<T> : IDisposableUI,LoopScrollPrefabSource, LoopScrollDataSource where T:UICellViewBase
//     {
//         private Transform mRoot;
//         private GameObject mPrefab;
//         private AssetHandle assetHandle;
//         private bool isLoadingPrefab;
//         private int mTotalDataCount = -1;
//         Stack<Transform> pool = new Stack<Transform>();
//         private Dictionary<Transform, T> _nodeToDataMap=new Dictionary<Transform, T>();
//         private LoopScrollRect mLoopScrollRect;
//         private Action<T, int> mBindDataCallback;
//         public UILoopScrollContainer(Transform mRootTrans, Action<T,int>  bindDataCallback)
//         {
//             mRoot = mRootTrans;
//             mBindDataCallback = bindDataCallback;
//             mLoopScrollRect = mRootTrans.GetComponent<LoopScrollRect>();
//             if (mLoopScrollRect == null)
//             {
//                 Debug.LogError(mRootTrans+" 节点上没有LoopScrollRect组件，后续没法正常工作");
//                 return;
//             }
//             mLoopScrollRect.prefabSource = this;
//             mLoopScrollRect.dataSource = this;
//         }
//         
//         
//         public void SetDataCount(int count)
//         {
//             if (mLoopScrollRect == null)
//             {
//                 return;
//             }
//             
//             mTotalDataCount = count;
//             if (mPrefab == null)
//             {
//                 if (isLoadingPrefab)
//                 {
//                     return;
//                 }
//                 else
//                 {
//                     var prefabPath = UISetting.GetUIPath<T>();
//                     isLoadingPrefab = true;
//                     ResourceManager.Instance.LoadAssetASync<GameObject>(prefabPath, (go,assetHandle) =>
//                     {
//                         mPrefab = go;
//                         this.assetHandle = (AssetHandle)assetHandle;
//                         isLoadingPrefab = false;
//                         DoRealSetCount();
//                     });
//                 }
//             }
//             else
//             {
//                 DoRealSetCount();
//             }
//         }
//
//         void DoRealSetCount()
//         {
//             mLoopScrollRect.totalCount = mTotalDataCount;
//             mLoopScrollRect.RefillCells();
//         }
//
//         public void JumpToIndex(int index,float time=0.1f)
//         {
//             mLoopScrollRect.ScrollToCellWithinTime(index,time);
//         }
//
//         public GameObject GetObject(int index)
//         {
//             GameObject cellGo;
//             if (pool.Count == 0)
//             {
//                 cellGo= GameObject.Instantiate(mPrefab);
//                 var cellInstance = Activator.CreateInstance<T>();
//                 cellInstance.InitView(cellGo);
//                 _nodeToDataMap[cellGo.transform] = cellInstance;
//                 return cellGo;
//             }
//             Transform poolGo = pool.Pop();
//             cellGo = poolGo.gameObject;
//             cellGo.SetActive(true);
//             return cellGo;
//         }
//
//         public void ReturnObject(Transform trans)
//         {
//             // Use `DestroyImmediate` here if you don't need Pool
//             trans.gameObject.SetActive(false);
//             trans.SetParent(mRoot, false);
//             pool.Push(trans);
//         }
//
//         public void ProvideData(Transform cellTran, int index)
//         {
//             if (_nodeToDataMap.TryGetValue(cellTran, out var cellInstance))
//             {
//                 if (mBindDataCallback != null)
//                 {
//                     mBindDataCallback.Invoke(cellInstance,index);
//                 }
//                 cellInstance.OnUpdateData(index);
//             }
//             else
//             {
//                 Debug.LogError($" sth is wrong with {cellTran} index {index}");
//             }
//         }
//
//         public void Dispose()
//         {
//             foreach (var kv in _nodeToDataMap)
//             {
//                 kv.Value.Dispose();
//             }
//             mPrefab = null;
//             pool.Clear();
//             
//         }
//     }
// }