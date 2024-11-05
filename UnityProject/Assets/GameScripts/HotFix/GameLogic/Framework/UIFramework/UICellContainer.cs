using System;
using System.Collections;
using System.Collections.Generic;
using Framework;
using UnityEngine;
using YooAsset;

namespace GameLogic
{
    public class  UICellContainer<T>:IDisposableUI where T:UICellViewBase
    {
        private Transform mRoot;
        private GameObject mPrefab;
        private bool isLoadingPrefab;
        private int mDataCount;
        private List<GameObject> mCellGoList;
        private List<T> mCellInstances;
        private T CellType;
        private AssetHandle assetHandle;
        private Action<T,int> mCellInitCallback;
        public UICellContainer(Transform mRootTrans, Action<T,int>  initCallBack)
        {
            mRoot = mRootTrans;
            mCellInitCallback = initCallBack;
        }

        public void Dispose()
        {
            if (mCellInstances != null)
            {
                foreach (var cellInstance in mCellInstances)
                {
                    cellInstance.Dispose();
                }
            }
        }
        
        
        public void SetDataCount(int dataCount) 
        {
            mDataCount = dataCount;
            if (mPrefab == null)
            {
                if (isLoadingPrefab)
                {
                    return;
                }
                else
                {
                    var prefabPath = UISetting.GetUIPath<T>();
                    isLoadingPrefab = true;
                    // ResourceManager.Instance.LoadAssetASync<GameObject>(prefabPath, (go,handleBase) =>
                    // {
                    //     mPrefab = go;
                    //     this.assetHandle = (AssetHandle)handleBase;  
                    //     isLoadingPrefab = false;
                    //    CreateCells();
                    // });
                }
            }
            else
            {
                CreateCells();
            }
        }

        void CreateCells()
        {
            if (mCellGoList == null)
            {
                mCellGoList = new List<GameObject>();
                mCellInstances = new List<T>();
            }
            else
            {
                for (int i = 0; i < mCellGoList.Count; i++)
                {
                    GameObject.Destroy(mCellGoList[i]);
                    mCellInstances[i].Dispose();
                }
                mCellGoList.Clear();
            }
            
            for (int i = 0; i < mDataCount; i++)
            {
                var cellGo = GameObject.Instantiate(mPrefab,mRoot);
                var cellInstance = Activator.CreateInstance<T>();
                cellInstance.InitView(cellGo);
                if (mCellInitCallback != null)
                {
                    mCellInitCallback.Invoke(cellInstance,i);
                }
                mCellGoList.Add(cellGo);
                mCellInstances.Add(cellInstance);
            }
        }
    }
}