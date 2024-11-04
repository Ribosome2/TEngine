using System.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    public class EventController
    {
        private readonly Dictionary<GlobalEvent, Dictionary<int, Delegate>> _eventMap = new Dictionary<GlobalEvent, Dictionary<int, Delegate>>();

        // public Dictionary<GlobalEvent, Delegate> EventMap => _eventMap;

        public void ClearUp()
        {
            _eventMap.Clear();
        }

        private bool CheckEventType(GlobalEvent globalEvent, Delegate handler)
        {
            // if (!_eventMap.TryGetValue(globalEvent, out var d)) return false;
            // if (d != null && d.GetType() != handler.GetType())
            // {
            //     throw new Exception($"操作事件 {globalEvent} 失败,事件参数类型不匹配，当前类型是{d.GetType().FullName}, 操作的类型是{handler.GetType().FullName}");
            // }
            //
            // return true;
            return true;
        }

        private void OnRemovingEvent(GlobalEvent globalEvent)
        {
            if (!_eventMap.TryGetValue(globalEvent, out var d)) return;
            if (d == null)
            {
                _eventMap.Remove(globalEvent);
            }
        }

        #region 添加监听

        public void AddListener(GlobalEvent globalEvent, Action handler, int id = -1)
        {
            var delegateMap = GetDelegateMap(globalEvent);
            if (!delegateMap.ContainsKey(id))
                delegateMap[id] = null;
            delegateMap[id] = (Action)delegateMap[id] + handler;
        }

        public void AddListener<T1>(GlobalEvent globalEvent, Action<T1> handler, int id = -1)
        {
            var delegateMap = GetDelegateMap(globalEvent);
            if (!delegateMap.ContainsKey(id))
                delegateMap[id] = null;
            delegateMap[id] = (Action<T1>)delegateMap[id] + handler;
        }

        public void AddListener<T1, T2>(GlobalEvent globalEvent, Action<T1, T2> handler, int id = -1)
        {
            var delegateMap = GetDelegateMap(globalEvent);
            if (!delegateMap.ContainsKey(id))
                delegateMap[id] = null;
            delegateMap[id] = (Action<T1, T2>)delegateMap[id] + handler;
        }

        public void AddListener<T1, T2, T3>(GlobalEvent globalEvent, Action<T1, T2, T3> handler, int id = -1)
        {
            var delegateMap = GetDelegateMap(globalEvent);
            if (!delegateMap.ContainsKey(id))
                delegateMap[id] = null;
            delegateMap[id] = (Action<T1, T2, T3>)delegateMap[id] + handler;
        }
        
        public void AddListener<T1, T2, T3, T4>(GlobalEvent globalEvent, Action<T1, T2, T3, T4> handler, int id = -1)
        {
            var delegateMap = GetDelegateMap(globalEvent);
            if (!delegateMap.ContainsKey(id))
                delegateMap[id] = null;
            delegateMap[id] = (Action<T1, T2, T3, T4>)delegateMap[id] + handler;
        }

        private Dictionary<int, Delegate> GetDelegateMap(GlobalEvent globalEvent, bool isNullable = false)
        {
            if (!_eventMap.TryGetValue(globalEvent, out var result) && !isNullable)
            {
                result = new Dictionary<int, Delegate>();
                _eventMap[globalEvent] = result;
            }

            return result;
        }

        #endregion

        #region 移除监听

        public void RemoveListener(GlobalEvent globalEvent, Action handler, int id = -1)
        {
            var delegateMap = GetDelegateMap(globalEvent, true);
            if (delegateMap == null || !delegateMap.ContainsKey(id))
                return;

            delegateMap[id] = (Action)delegateMap[id] - handler;
            if (delegateMap[id] == null)
                delegateMap.Remove(id);
        }

        public void RemoveListener<T1>(GlobalEvent globalEvent, Action<T1> handler, int id = -1)
        {
            var delegateMap = GetDelegateMap(globalEvent, true);
            if (delegateMap == null || !delegateMap.ContainsKey(id))
                return;

            delegateMap[id] = (Action<T1>)delegateMap[id] - handler;
            if (delegateMap[id] == null)
                delegateMap.Remove(id);
        }

        public void RemoveListener<T1, T2>(GlobalEvent globalEvent, Action<T1, T2> handler, int id = -1)
        {
            var delegateMap = GetDelegateMap(globalEvent, true);
            if (delegateMap == null || !delegateMap.ContainsKey(id))
                return;

            delegateMap[id] = (Action<T1, T2>)delegateMap[id] - handler;
            if (delegateMap[id] == null)
                delegateMap.Remove(id);
        }

        public void RemoveListener<T1, T2, T3>(GlobalEvent globalEvent, Action<T1, T2, T3> handler, int id = -1)
        {
            var delegateMap = GetDelegateMap(globalEvent, true);
            if (delegateMap == null || !delegateMap.ContainsKey(id))
                return;

            delegateMap[id] = (Action<T1, T2, T3>)delegateMap[id] - handler;
            if (delegateMap[id] == null)
                delegateMap.Remove(id);
        }
        
        public void RemoveListener<T1, T2, T3, T4>(GlobalEvent globalEvent, Action<T1, T2, T3, T4> handler, int id = -1)
        {
            var delegateMap = GetDelegateMap(globalEvent, true);
            if (delegateMap == null || !delegateMap.ContainsKey(id))
                return;

            delegateMap[id] = (Action<T1, T2, T3, T4>)delegateMap[id] - handler;
            if (delegateMap[id] == null)
                delegateMap.Remove(id);
        }

        public void RemoveListenerById(int id)
        {
            foreach (var iter in _eventMap.Values)
            {
                iter.Remove(id);
            }
        }

        #endregion

        #region 触发事件

        public void FireEvent(GlobalEvent globalEvent)
        {
            if (!_eventMap.TryGetValue(globalEvent, out var d))
                return;

            foreach (var key in d.Keys.ToArray())
            {
                if (d.TryGetValue(key, out var temp))
                {
                    if (temp != null)
                        ((Action)temp)();
                }
            }
        }


        public void FireEvent<T1>(GlobalEvent globalEvent, T1 arg1)
        {
            if (!_eventMap.TryGetValue(globalEvent, out var d))
                return;

            foreach (var key in d.Keys.ToArray())
            {
                if (d.TryGetValue(key, out var temp))
                {
                    if (temp != null)
                        ((Action<T1>)temp)(arg1);
                }
            }
        }

        public void FireEvent<T1, T2>(GlobalEvent globalEvent, T1 arg1, T2 arg2)
        {
            if (!_eventMap.TryGetValue(globalEvent, out var d))
                return;

            foreach (var key in d.Keys.ToArray())
            {
                if (d.TryGetValue(key, out var temp))
                {
                    if (temp != null)
                        ((Action<T1, T2>)temp)(arg1, arg2);
                }
            }
        }

        public void FireEvent<T1, T2, T3>(GlobalEvent globalEvent, T1 arg1, T2 arg2, T3 arg3)
        {
            if (!_eventMap.TryGetValue(globalEvent, out var d))
                return;

            foreach (var key in d.Keys.ToArray())
            {
                if (d.TryGetValue(key, out var temp))
                {
                    if (temp != null)
                        ((Action<T1, T2, T3>)temp)(arg1, arg2, arg3);
                }
            }
        }
        
        public void FireEvent<T1, T2, T3, T4>(GlobalEvent globalEvent, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            if (!_eventMap.TryGetValue(globalEvent, out var d))
                return;

            foreach (var key in d.Keys.ToArray())
            {
                if (d.TryGetValue(key, out var temp))
                {
                    if (temp != null)
                        ((Action<T1, T2, T3, T4>)temp)(arg1, arg2, arg3, arg4);
                }
            }
        }

        #endregion
    }
}
