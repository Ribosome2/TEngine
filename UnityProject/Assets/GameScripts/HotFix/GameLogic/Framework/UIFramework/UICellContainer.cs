using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Framework;
using TEngine;
using UnityEngine;
using YooAsset;

namespace GameLogic
{
    public class  UICellContainer<T>:IDisposableUI where T:UICellViewBase
    {
        private Transform mRoot;
        private bool isLoadingPrefab;
        private int mDataCount;
        private List<GameObject> mCellGoList;
        private List<T> mCellInstances;
        private T CellType;
        private Action<T,int> mCellInitCallback;
        private bool isPrefabLoaded = false;
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
        
        
        public async Task SetDataCount(int dataCount) 
        {
            mDataCount = dataCount;
            //todo：支持异步加载
            // if (isPrefabLoaded)
            // {
            //     if (isLoadingPrefab)
            //     {
            //         return;
            //     }
            //     else
            //     {
            //         var prefabPath = UISetting.GetUIPath<T>();
            //         isLoadingPrefab = true;
            //         var go =await GameModule.Resource.LoadGameObjectAsync(prefabPath,mRoot,default,"");
            //         isLoadingPrefab = false;
            //        CreateCells();
            //     }
            // }
            // else
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
                var prefabPath = UISetting.GetUIPath<T>();
                var cellGo =GameModule.Resource.LoadGameObject(prefabPath,mRoot);
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