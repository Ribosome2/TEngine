using UnityEngine;

namespace GameLogic
{
    public class MonsterUnit:GameUnit
    {
        public int Hp=100;
        public float lastHitTime;
        public bool IsHitDirty;
        private bool IsDead = false;

        public void ApplyHit(int hit)
        {
            Hp -= hit;
            IsHitDirty = true;
            lastHitTime = Time.realtimeSinceStartup;
            if (Hp <= 0)
            {
                IsDead = true;
                SetAsToDelete();
            }
        }
        
        public bool GetIsDead()
        {
            return IsDead;
        }
    }
}