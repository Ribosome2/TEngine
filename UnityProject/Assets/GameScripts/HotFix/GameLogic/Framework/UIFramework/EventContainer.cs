using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 事件容器，自动管理订阅关系
    /// 目前只支持没有参数的时间，多个参数事件，UnSubscribe的时候委托类型映射有点麻烦！！
    /// </summary>
    public class EventContainer : IDisposable
    {
        // 记录所有订阅的事件类型和处理函数（委托）
        private readonly List<(GlobalEvent Event, Delegate Handler)> _subscriptions_T0 = new List<(GlobalEvent, Delegate)>();
        private readonly List<(GlobalEvent Event, Delegate Handler)> _subscriptions_T1_bool = new List<(GlobalEvent, Delegate)>();
        private readonly List<(GlobalEvent Event, Delegate Handler)> _subscriptions_T1_float = new List<(GlobalEvent, Delegate)>();
        #region 订阅方法（自动跟踪）

        /// <summary>
        /// 订阅无参数事件
        /// </summary>
        public void Subscribe(GlobalEvent globalEvent, Action handler)
        {
            _subscriptions_T0.Add((globalEvent, handler));
            EventCenter.Subscribe(globalEvent, handler);
        }
        public  void Subscribe<T1>(GlobalEvent globalEvent, Action<T1> handler, int id = -1)
        {
            if (typeof(T1) == typeof(Boolean))
            {
                _subscriptions_T1_bool.Add((globalEvent, handler));
            }
            else  if (typeof(T1) == typeof(float))
            {
                _subscriptions_T1_float.Add((globalEvent, handler));
            }
            else
            {
                Debug.LogError($"暂未支持参数类型 {typeof(T1)}");
                return;
            }

            EventCenter.Subscribe(globalEvent, handler, id);
        }


        #endregion

        #region 清除订阅

        /// <summary>
        /// 清除所有订阅的事件
        /// </summary>
        private void ClearEvents()
        {
            // 反向遍历避免集合修改异常
            for (int i = _subscriptions_T0.Count - 1; i >= 0; i--)
            {
                var (globalEvent, handler) = _subscriptions_T0[i];
                EventCenter.UnSubscribe(globalEvent, (System.Action)handler, -1);
            }

            for (int i = _subscriptions_T1_bool.Count - 1; i >= 0; i--)
            {
                var (globalEvent, handler) = _subscriptions_T1_bool[i];
                EventCenter.UnSubscribe(globalEvent, (System.Action<bool>)handler, -1);
            }

            for (int i = _subscriptions_T1_float.Count - 1; i >= 0; i--)
            {
                var (globalEvent, handler) = _subscriptions_T1_float[i];
                EventCenter.UnSubscribe(globalEvent, (System.Action<bool>)handler, -1);
            }
        }

        /// <summary>
        /// 释放资源（自动清除订阅）
        /// </summary>
        public void Dispose()
        {
            ClearEvents();
        }

        #endregion
    }
}
