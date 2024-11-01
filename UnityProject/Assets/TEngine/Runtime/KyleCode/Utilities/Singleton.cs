using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatchCode
{
    public static class Singleton
    {
        static private List<ISingleton> _singletons = new List<ISingleton>();
        static private List<ISingleton> _destroyableSingletons = new List<ISingleton>();

        public static void AddSingle<T>(T singleton) where T : Singleton<T>, new()
        {
            _singletons.Add(singleton);
            if (typeof(T).GetCustomAttributes(typeof(PermanentAttribute), false).Length == 0)
                _destroyableSingletons.Add(singleton);
        }

        public static void Clear()
        {
            foreach (var iter in _destroyableSingletons)
            {
                iter.Destroy();
                _singletons.Remove(iter);
            }
            _destroyableSingletons.Clear();
        }
    }

    public interface ISingleton
    {
        void Destroy();
    }

    public class Singleton<T> : ISingleton where T : Singleton<T>, new()
    {
        protected static T instance = null;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new T();
                    instance.Init();
                    Singleton.AddSingle(instance);
                }

                return instance;
            }
        }

        public static void StaticDestroy()
        {
            instance?.Destroy();
        }

        protected virtual void Init()
        {

        }

        public void Destroy()
        {
            instance.OnDestroy();
            instance = null;
        }

        protected virtual void OnDestroy()
        {

        }
    }
}
