using System;
using System.Collections.Generic;
using GameBase;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameLogic
{
    public class ZombieBattleMgr:Singleton<ZombieBattleMgr>
    {
        private List<MonsterUnit> _monsterUnits = new List<MonsterUnit>();
        private List<GameUnit> _bulletUnits = new List<GameUnit>();
        private List<GameUnit> _playerUnits = new List<GameUnit>();
        private float spawnInterval = 1;
        private float spwawTimer = 1;
        public PlayerAttribute Attributes=new PlayerAttribute();
      

        public int MaxHp=3000;
        public int Hp=3000;
        private SpriteRenderer hpSprite;
        private Queue<GameUnit> deleteQueue = new Queue<GameUnit>();
        
        
        public void Start()
        {
            Attributes = new PlayerAttribute();
            hpSprite = GameObject.Find("gameHp").GetComponent<SpriteRenderer>();
            CreatePlayerUnit();
            UnityMessage.Instance.OnGameDrawGizmos += OnDrawGizmos;
        }
        
        public void Dispose()
        {
            foreach (var unit in _monsterUnits)
            {
                unit.Dispose();
            }
            _monsterUnits.Clear();
            Hp = MaxHp;
            hpSprite = null;
            UnityMessage.Instance.OnGameDrawGizmos -= OnDrawGizmos;
        }

        private void OnDrawGizmos()
        {
            foreach (var unit in _bulletUnits)
            {
                unit.OnDrawGizmos();
            }
            
            foreach (var unit in _playerUnits)
            {
                unit.OnDrawGizmos();
            }
        }

        public void SetSpawnInterval(float interval)
        {
            spawnInterval = interval;
        }
        
        public void Update()
        {
            spwawTimer -= Time.deltaTime;
            if (spwawTimer <= 0)
            {
                spwawTimer = spawnInterval;
                SpawnMonsterUnit();
            }

            UpdateMonsterUnits();
            foreach (var unit in _playerUnits)
            {
                unit.Update();
            }
            
            UpdateBullet();
        }

        private void UpdateMonsterUnits()
        {
            foreach (var unit in _monsterUnits)
            {
                if (unit.GetIsToDelete() || unit.GetIsDead())
                {
                    deleteQueue.Enqueue(unit);
                }
                else
                {
                    unit.Update();
                }
            }

            while (deleteQueue.Count>0)
            {
                var unit = deleteQueue.Dequeue();
                _monsterUnits.Remove((MonsterUnit)unit);
                unit.Dispose();
            }
        }

        private void UpdateBullet()
        {
            foreach (var unit in _bulletUnits)
            {
                if (unit.GetIsToDelete())
                {
                    deleteQueue.Enqueue(unit);
                }
                else
                {
                    unit.Update();
                }
            }

            CheckDeleteQueue();
        }

        private void CheckDeleteQueue()
        {
            while (deleteQueue.Count>0)
            {
                var unit = deleteQueue.Dequeue();
                _bulletUnits.Remove(unit);
                unit.Dispose();
            }
        }

        void CreatePlayerUnit()
        {
            GameUnit playerUnit = new GameUnit();
            // playerUnit.AddComponent<NormalGunComponent>();
            // playerUnit.AddComponent<DirectionalLaserComponent>();
            // playerUnit.AddComponent<LaserRenderer>();
            playerUnit.AddComponent<LockLaserComponent>();
            playerUnit.AddComponent<LockLaserRenderer>();
            _playerUnits.Add(playerUnit);
        }

        GameUnit SpawnMonsterUnit()
        {
            var monsterUnit = Activator.CreateInstance<MonsterUnit>();
            var moveComponent = monsterUnit.AddComponent<MonsterMoveComponent>();
            monsterUnit.AddComponent<Collider2DComponent>();
            monsterUnit.AddComponent<MonsterRenderer>();
            moveComponent.Pos = new Vector3(Random.Range(-2.3f, 2.3f), 0, 4.6f);
            _monsterUnits.Add(monsterUnit);
            return monsterUnit;
        }

        public void HandleZombieDamage(int damage)
        {
            Hp -= damage;
            if (Hp <= 0)
            {
                Hp = 0;
            }
            EventCenter.Fire(GlobalEvent.ZombieEatHealthEvent);
            hpSprite.size = new Vector2(3.77f * Hp / MaxHp, hpSprite.size.y);
        }




        public int GetUnitCount()
        {
            return _monsterUnits.Count;
        }

        public GameUnit GetShootInitTarget(Vector3 gunStartPoint)
        {
            float minDist = float.MaxValue;
            GameUnit target = null;
            foreach (var unit in _monsterUnits)
            {
                var dist =BattleMathUtil.GetXZSquareMagnitude(unit.GetComponent<MonsterMoveComponent>().Pos,gunStartPoint);
                if (dist < minDist)
                {
                    minDist = dist;
                    target = unit;
                }
            }
            return target;
        }

        public  GameUnit CreateDirectionalBullet(Vector3 startPos, Vector3 dir)
        {
            var unit = BulletFactory.CreateDirectionalBullet(startPos, dir);
            _bulletUnits.Add(unit);
            return unit;
        }

        
        public List<MonsterUnit> GetAllMonsterUnits()
        {
            return _monsterUnits;
        }
    }
}