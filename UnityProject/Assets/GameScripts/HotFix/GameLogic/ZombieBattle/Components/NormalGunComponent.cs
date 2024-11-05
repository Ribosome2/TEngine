using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 玩家的普攻枪
    /// </summary>
    public class NormalGunComponent:GameComponent
    {
        public float ShootInterval=0.5f;
        private float shootCd = 0;
        public Vector3 gunStartPoint = new Vector3(0f, 0.0f, -4.18f);
        public override void OnUpdate()
        {
            base.OnUpdate();
            if (shootCd <= 0)
            {
                if (ShootBullet())
                {
                    shootCd = ShootInterval;
                }
                else
                {
                    shootCd = 0;
                }
            }

            shootCd -= Time.deltaTime;
        }

        public bool ShootBullet()
        {
            GameUnit getClosestTarget = ZombieBattleMgr.Instance.GetShootInitTarget(gunStartPoint);
            if (getClosestTarget != null)
            {
                var targetPos = getClosestTarget.GetComponent<MonsterMoveComponent>().Pos;
                var dir = targetPos - gunStartPoint;
                dir.y = 0;
                ZombieBattleMgr.Instance.CreateDirectionalBullet(gunStartPoint, dir);
                return true;
                
            }
            return false;
        }
    }
}