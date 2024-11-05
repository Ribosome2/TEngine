using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    public class UIViewBase : IDisposableUI

    {
        private List<IDisposableUI> mCellContainers;
        private List<IDisposableUI> mLoopScrollContainers;
        public GameObject ViewGo
        {
            get { return _gameObject; }
        }

        protected BindObjectMono RefBind;
        protected GameObject _gameObject;

        public void InitView(GameObject go)
        {
            _gameObject = go;
            RefBind = go.GetComponent<BindObjectMono>();
            OnCreate();
            OnBindUIEvent();

        }

        protected virtual void OnCreate()
        {

        }

        protected virtual void OnDestroy()
        {
            
        }

        protected virtual void OnBindUIEvent()
        {

        }

        public void Dispose()
        {
            if (mCellContainers != null)
            {
                foreach (var cellContainer in mCellContainers)
                {
                    cellContainer.Dispose();
                }
            }
            
            if (mLoopScrollContainers != null)
            {
                foreach (var cellContainer in mLoopScrollContainers)
                {
                    cellContainer.Dispose();
                }
            }
            OnDestroy();
        }
        
        
        protected UICellContainer<T> CreateCellContainer<T>(Transform cellRoot, Action<T,int> initCallBack) where T:UICellViewBase
        {
            if (mCellContainers == null)
            {
                mCellContainers = new List<IDisposableUI>();
            }
            
            var container = new UICellContainer<T>(cellRoot,initCallBack);
            mCellContainers.Add(container);
            return container;
        }
        
        // protected UILoopScrollContainer<T> CreateLoopScrollContainer<T>(Transform cellRoot, Action<T,int> bindDataCallBack)
        //     where T:UICellViewBase
        // {
        //     if (mLoopScrollContainers == null)
        //     {
        //         mLoopScrollContainers = new List<IDisposableUI>();
        //     }
        //     
        //     var container = new UILoopScrollContainer<T>(cellRoot,bindDataCallBack);
        //     mLoopScrollContainers.Add(container);
        //     return container;
        // }
        
        
    }
}