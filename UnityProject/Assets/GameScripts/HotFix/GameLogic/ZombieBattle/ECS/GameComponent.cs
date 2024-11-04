using UnityEngine;

namespace GameLogic
{
    public class GameComponent
    {
        protected GameUnit mOwner;
        public bool isInited = false;
        public void SetOwner(GameUnit owner)
        {
            mOwner = owner;
        }
        public virtual void OnInit()
        {
            
        }


        public virtual void OnUpdate()
        {
            
        }

        public virtual void Dispose()
        {
        }
        public T GetComponent<T>() where T:GameComponent
        {
            return mOwner.GetComponent<T>();
        }

        public void CheckInit()
        {
            if (isInited == false)
            {
                isInited = true;
                OnInit();
            }
        }

        public virtual void OnDrawGizmos()
        {
        }
    }
}