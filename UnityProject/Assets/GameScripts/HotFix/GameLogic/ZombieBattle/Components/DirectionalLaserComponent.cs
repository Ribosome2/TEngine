using UnityEngine;

namespace GameLogic
{
    public class DirectionalLaserComponent : GameComponent
    {
        public float ShootInterval = 3.5f;
        private float shootCd = 0;
        public Vector3 laserStartPoint = new Vector3(0.55f, 0.0f, -4.08f);
        private int ApplyDamageCount = 3;
        private int DamageCounter = 0;
        public float DamageInterval = 0.3f;

        public bool isShooting = false;
        public Vector3 ShootDirection;
        public float DamageTimer = 0;

        public float Width = 0.2f;
        public float Height = 10;
        public Vector3 shootCenter;
        public Vector3 LaserEndPos;
        public int ShootIndex = 0;

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (isShooting)
            {
                UpdateShootState();
            }
            else
            {
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
                else
                {
                    shootCd -= Time.deltaTime;
                }
            }
        }

        private void UpdateShootState()
        {
            if (DamageCounter < ApplyDamageCount)
            {
                if (DamageTimer <= 0)
                {
                    DamageTimer += DamageInterval;
                    DamageCounter += 1;
                    CheckDamage();
                }

                DamageTimer -= Time.deltaTime;
            }
            else
            {
                isShooting = false;
                DamageTimer = 0;
                shootCd = ShootInterval;
                DamageCounter = 0;
            }
        }

        private void CheckDamage()
        {
            Debug.Log("CheckDamage");
            var allMonsters = ZombieBattleMgr.Instance.GetAllMonsterUnits();
            var center = shootCenter;
            var bulletRect = new CollisionRect(new Vector2(center.x, center.z), Width, Height,
                new Vector2(ShootDirection.x, ShootDirection.z));
            foreach (MonsterUnit monsterUnit in allMonsters)
            {
                var colliderComp = monsterUnit.GetComponent<Collider2DComponent>();
                var monsterPos = monsterUnit.GetComponent<MonsterMoveComponent>().Pos;
                var monsterRect =
                    new CollisionRect(new Vector2(monsterPos.x, monsterPos.z + colliderComp.Height * 0.5f),
                        colliderComp.Width, colliderComp.Height, new Vector2(0, 1));
                if (CollisionMathUtil.IsIntersect(bulletRect, monsterRect))
                {
                    monsterUnit.ApplyHit(15);
                    // Debug.Log("Damage--- " + monsterUnit);
                }
            }
        }

        public bool ShootBullet()
        {
            var allMonsters = ZombieBattleMgr.Instance.GetAllMonsterUnits();
            if (allMonsters.Count > 0)
            {
                var target = allMonsters[Random.Range(0, allMonsters.Count)];
                var targetPos = target.GetComponent<MonsterMoveComponent>().Pos;
                var dir = targetPos - laserStartPoint;
                dir.y = 0;
                dir = dir.normalized;
                shootCenter = laserStartPoint + dir * (Height * 0.5f);
                LaserEndPos = laserStartPoint + dir * Height;
                ShootIndex++;
                ShootDirection = dir;
                isShooting = true;
                return true;
            }

            return false;
        }

        public override void OnDrawGizmos()
        {
            var center = shootCenter;
            var bulletRect = new CollisionRect(new Vector2(center.x, center.z), Width, Height,
                new Vector2(ShootDirection.x, ShootDirection.z));
            CollisionDebugUtil.DrawRectCorners(bulletRect, shootCenter.y, 0.05f);
            return;
            var allMonsters = ZombieBattleMgr.Instance.GetAllMonsterUnits();

            foreach (MonsterUnit monsterUnit in allMonsters)
            {
                var colliderComp = monsterUnit.GetComponent<Collider2DComponent>();
                var monsterPos = monsterUnit.GetComponent<MonsterMoveComponent>().Pos;
                var monsterRect = new CollisionRect(new Vector2(monsterPos.x, monsterPos.z), colliderComp.Width,
                    colliderComp.Height, new Vector2(0, 1));
                CollisionDebugUtil.DrawRectCorners(monsterRect, monsterPos.y, 0.05f);
                if (CollisionMathUtil.IsIntersect(bulletRect, monsterRect))
                {
                    Gizmos.DrawCube(monsterPos, new Vector3(0.1f, 3, 0.1f));
                }
            }
        }
    }
}