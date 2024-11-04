using UnityEngine;

namespace GameLogic
{
    public class LockLaserComponent : GameComponent
    {
        public float ShootInterval = 3.5f;
        private float shootCd = 0;
        public Vector3 laserStartPoint = new Vector3(0.55f, 0.0f, -4.08f);
        private int ApplyDamageCount = 5;
        private int DamageCounter = 0;
        public float DamageInterval = 0.3f;

        public bool isShooting = false;
        public Vector3 ShootDirection;
        public float DamageTimer = 0;

        public Vector3 LaserEndPos;
        public int ShootIndex = 0;
        private MonsterUnit mLockMonster;

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
                UpdateShootTarget(mLockMonster);
                if (mLockMonster == null || mLockMonster.GetIsDead())
                {
                    var allMonsters = ZombieBattleMgr.Instance.GetAllMonsterUnits();
                    if (allMonsters.Count > 0)
                    {
                        var target = allMonsters[Random.Range(0, allMonsters.Count)];
                        UpdateShootTarget(target);
                    }
                    else
                    {
                        StopShooting();
                    }
                }
            }
            else
            {
                StopShooting();
            }
        }

        private void StopShooting()
        {
            isShooting = false;
            DamageTimer = 0;
            shootCd = ShootInterval;
            DamageCounter = 0;
            mLockMonster = null;
        }

        private void CheckDamage()
        {
            Debug.Log("CheckDamage");
            if (mLockMonster != null)
            {
                mLockMonster.ApplyHit(75);
            }
                  
        }

        public bool ShootBullet()
        {
            var allMonsters = ZombieBattleMgr.Instance.GetAllMonsterUnits();
            if (allMonsters.Count > 0)
            {
                var target = allMonsters[Random.Range(0, allMonsters.Count)];
                UpdateShootTarget(target);
                isShooting = true;
                return true;
            }

            return false;
        }

        private void UpdateShootTarget(MonsterUnit target)
        {
            mLockMonster = target;
            var targetPos = target.GetComponent<MonsterMoveComponent>().Pos;
            var dir = targetPos - laserStartPoint;
            dir.y = 0;
            dir = dir.normalized;
            LaserEndPos = targetPos;
            ShootDirection = dir;
            ShootIndex++;
        }
    }
}