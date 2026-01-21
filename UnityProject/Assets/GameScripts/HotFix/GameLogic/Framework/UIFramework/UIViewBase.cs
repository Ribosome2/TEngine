using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    public class UIViewBase : IDisposableUI

    {
        private List<IDisposableUI> mCellContainers;
        private List<IDisposableUI> mLoopScrollContainers;
        private bool isDispose = false;
        public GameObject ViewGo
        {
            get { return _gameObject; }
        }

        protected BindObjectMono RefBind;
        protected GameObject _gameObject;



        public virtual void InitView(GameObject go)
        {
            _gameObject = go;
            OnAfterGameObjectInit();
            RefBind = go.GetComponent<BindObjectMono>();
            InitWidgetBind();
            OnCreate();
            OnBindUIEvent();

        }

        protected virtual void OnAfterGameObjectInit()
        {

        }

        public bool IsDispose()
        {
            return isDispose;
        }

        protected virtual void InitWidgetBind()
        {

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

        public virtual void Dispose()
        {
            isDispose = true;
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


        /// <summary>
        /// 创建挂CellView容器
        /// 通过设置SetDataCount 会自动创建出对应数量个CellView
        /// </summary>
        /// <param name="cellRoot">挂载的父节点</param>
        /// <param name="initCallBack">每个CellView和初始化回调，第一个参数是CellView类型，第二个是CellView的索引</param>
        /// <typeparam name="T">容器Cell类型</typeparam>
        /// <returns></returns>

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

        protected UILoopScrollContainer<T> CreateLoopScrollContainer<T>(Transform cellRoot, Action<T,int> bindDataCallBack)
            where T:UICellViewBase
        {
            if (mLoopScrollContainers == null)
            {
                mLoopScrollContainers = new List<IDisposableUI>();
            }

            var container = new UILoopScrollContainer<T>(cellRoot,bindDataCallBack);
            mLoopScrollContainers.Add(container);
            return container;
        }


    }
}
