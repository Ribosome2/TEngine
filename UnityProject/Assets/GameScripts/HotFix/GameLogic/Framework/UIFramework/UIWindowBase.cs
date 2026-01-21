using System;
using System.Collections.Generic;
using TEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic
{
    public abstract class UIWindowBase:UIViewBase
    {
        public E_UIBackgroundType BackgroundType { get; set; } = E_UIBackgroundType.None;
        private RenderTexture blurRt ;
        protected GameObject _bgrMaskGo;
        
        /// <summary>
        /// 无参数的事件管理器，只需要SubScribe，自动U
        /// </summary>
        protected EventContainer _eventContainer = new EventContainer();
        #region 生命周期函数

        public virtual void OnClose()
        {
            
        }

        public virtual void OpenWithParam<T>(T arg)
        {
            
        }


        #endregion

        protected override void OnAfterGameObjectInit()
        {
            base.OnAfterGameObjectInit();
            if (BackgroundType == E_UIBackgroundType.BlurMask)
            {
                var blurMask = "Assets/GameAssets/Prefabs/UI/CommonUI/BlurBgrMask.prefab";
                _bgrMaskGo = GameModule.Resource.LoadGameObject(blurMask, _gameObject.transform);
                blurRt=UIManager.Instance.BlurScreenManager.GetBlurTextureCopy();
                _bgrMaskGo.transform.SetAsFirstSibling();
                _bgrMaskGo.GetComponent<RawImage>().texture = blurRt;
            }
        }
        public override void Dispose()
        {
            _eventContainer?.Dispose();
            base.Dispose();
            if (blurRt)
            {
                UIManager.Instance.BlurScreenManager.ReleaseRT(blurRt);
                blurRt = null;
            }
        }

        protected void CloseThisWindow()
        {
            UIManager.Instance.CloseWindow(this);
        }

  

        public virtual RectTransform GetGuideObject(string name)
        {
            var widget = RefBind.GetWidgetWrap(name);
            if (widget == null)
            {
                Debug.LogError($"$窗口{this.GetType().Name}没有节点{name}");
                return null;
            }
            return widget.RectTransform;
        }

        protected string forceOnlyOp = "";
        public virtual void ForceOnlyOp(string op)
        {
            forceOnlyOp = op;
        }
    }
}