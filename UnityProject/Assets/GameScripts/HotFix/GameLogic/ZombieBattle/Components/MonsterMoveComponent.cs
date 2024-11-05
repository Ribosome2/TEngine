
using UnityEngine;

namespace GameLogic
{
    public class MonsterMoveComponent:GameComponent
    {
        ///坐标为怪物的底部
        public Vector3 Pos;
        public float Speed = 0.5f;
        private const float minZ = -3.25f;
        private const float DAMAGE_INTERVAL = 0.5f;
        private float damageTimer = 0;

        public override void OnUpdate()
        {
            base.OnUpdate();
            var posZ = Pos.z - (Speed * Time.deltaTime);
            if (posZ <= minZ)
            {
                posZ = minZ;
                damageTimer += Time.deltaTime;
                if (damageTimer > DAMAGE_INTERVAL)
                {
                    damageTimer = 0;
                    ZombieBattleMgr.Instance.HandleZombieDamage(1);
                }
            }
            else
            {
                damageTimer = 0;
            }
            Pos = new Vector3(Pos.x,Pos.y, posZ);
        }
    }
}