using System;

namespace GameLogic
{
    public class EventCenter
    {
        private static readonly EventController EventController = new EventController();

        #region 增加监听

        public static void Subscribe(GlobalEvent globalEvent, Action handler, int id = -1)
        {
            EventController.AddListener(globalEvent, handler, id);
        }

        public static void Subscribe<T1>(GlobalEvent globalEvent, Action<T1> handler, int id = -1)
        {
            EventController.AddListener(globalEvent, handler, id);
        }

        public static void Subscribe<T1, T2>(GlobalEvent globalEvent, Action<T1, T2> handler, int id = -1)
        {
            EventController.AddListener(globalEvent, handler, id);
        }

        public static void Subscribe<T1, T2, T3>(GlobalEvent globalEvent, Action<T1, T2, T3> handler, int id = -1)
        {
            EventController.AddListener(globalEvent, handler, id);
        }
        
        public static void Subscribe<T1, T2, T3, T4>(GlobalEvent globalEvent, Action<T1, T2, T3, T4> handler, int id = -1)
        {
            EventController.AddListener(globalEvent, handler, id);
        }

        #endregion

        #region 移除监听

        public static void UnSubscribe(GlobalEvent globalEvent, Action handler, int id = -1)
        {
            EventController.RemoveListener(globalEvent, handler, id);
        }

        public static void UnSubscribe<T1>(GlobalEvent globalEvent, Action<T1> handler, int id = -1)
        {
            EventController.RemoveListener(globalEvent, handler, id);
        }

        public static void UnSubscribe<T1, T2>(GlobalEvent globalEvent, Action<T1, T2> handler, int id = -1)
        {
            EventController.RemoveListener(globalEvent, handler, id);
        }

        public static void UnSubscribe<T1, T2, T3>(GlobalEvent globalEvent, Action<T1, T2, T3> handler, int id = -1)
        {
            EventController.RemoveListener(globalEvent, handler, id);
        }
        
        public static void UnSubscribe<T1, T2, T3, T4>(GlobalEvent globalEvent, Action<T1, T2, T3, T4> handler, int id = -1)
        {
            EventController.RemoveListener(globalEvent, handler, id);
        }

        public static void UnSubscribeById(int id)
        {
            EventController.RemoveListenerById(id);
        }

        #endregion

        #region 触发事件

        public static void Fire(GlobalEvent globalEvent)
        {
            EventController.FireEvent(globalEvent);
        }

        public static void Fire<T1>(GlobalEvent globalEvent, T1 arg1)
        {
            EventController.FireEvent(globalEvent, arg1);
        }

        public static void Fire<T1, T2>(GlobalEvent globalEvent, T1 arg1, T2 arg2)
        {
            EventController.FireEvent(globalEvent, arg1, arg2);
        }

        public static void Fire<T1, T2, T3>(GlobalEvent globalEvent, T1 arg1, T2 arg2, T3 arg3)
        {
            EventController.FireEvent(globalEvent, arg1, arg2, arg3);
        }
        
        public static void Fire<T1, T2, T3, T4>(GlobalEvent globalEvent, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            EventController.FireEvent(globalEvent, arg1, arg2, arg3, arg4);
        }

        #endregion
    }
}
