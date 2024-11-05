using System;
using System.Collections.Generic;

namespace GameLogic
{
    public class GameUnit
    {
        private Dictionary<Type, GameComponent> mComponentList=new Dictionary<Type, GameComponent>() ;
        private bool toDelete = false;

        public T AddComponent<T>() where T:GameComponent
        {
            var comp =ECSHelper.AddComponent<T>(this);
            mComponentList.Add(typeof(T), comp);
            comp.SetOwner(this);

            return comp;
        }

        public T GetComponent<T>() where T:GameComponent
        {
            if (mComponentList.TryGetValue(typeof(T),out GameComponent result))
            {
                return (T)result;
            }

            return null;
        }

        public void Update()
        {
            foreach (var kv in mComponentList)
            {
                kv.Value.CheckInit();
                kv.Value.OnUpdate();
            }
        }

        public void Dispose()
        {
            foreach (var kv in mComponentList)
            {
                kv.Value.Dispose();
            }
        }

        public void SetAsToDelete()
        {
            this.toDelete = true;
        }

        public bool GetIsToDelete()
        {
            return toDelete;
        }

        public void OnDrawGizmos()
        {
            foreach (var kv in mComponentList)
            {
                kv.Value.OnDrawGizmos();
            }
        }
    }
}