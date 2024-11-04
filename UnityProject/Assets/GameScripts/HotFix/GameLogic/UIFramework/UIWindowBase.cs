using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    public abstract class UIWindowBase:UIViewBase
    {
       
        #region 生命周期函数

        public virtual void OnClose()
        {
            
        }

        public virtual void OpenWithParam<T>(T arg)
        {
            
        }
        #endregion
        
        protected void CloseThisWindow()
        {
            UIManager.Instance.CloseWindow(this);
        }

        public bool NeedCloseOnSwitchScene()
        {
            return true;
        }
       
    }
}