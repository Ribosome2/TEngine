using UnityEngine;

namespace GameLogic
{
    public class DirectionalBulletComponent:GameComponent
    {
        public Vector3 Pos;
        public Vector3 MoveDir;
        public float moveSpeed = 2.3f;

        public float Width = 0.14f;
        public float Height = 0.32f;
        
        public float MinX = -3f;
        public float MaxX = 3f;
        public float MaxZ = 6f;
        public override void OnUpdate()
        {
            base.OnUpdate();
            var moveDt = moveSpeed * Time.deltaTime;
            Pos += MoveDir * moveDt;
            if (Pos.x < MinX || Pos.x > MaxX || Pos.z > MaxZ)
            {
                mOwner.SetAsToDelete();
            }
            else
            {
                CheckHitMonsters();
            }
        }

        void CheckHitMonsters()
        {
            var allMonsters = ZombieBattleMgr.Instance.GetAllMonsterUnits();
            var center = Pos;
            var bulletRect = new CollisionRect(new Vector2(center.x,center.z),Width,Height,new Vector2(MoveDir.x,MoveDir.z));
            foreach (MonsterUnit monsterUnit in allMonsters)
            {
                var colliderComp = monsterUnit.GetComponent<Collider2DComponent>();
                var monsterPos = monsterUnit.GetComponent<MonsterMoveComponent>().Pos;
                var monsterRect  = new CollisionRect(new Vector2(monsterPos.x,monsterPos.z+colliderComp.Height*0.5f),colliderComp.Width,colliderComp.Height,new Vector2(0,1));
                if (CollisionMathUtil.IsIntersect(bulletRect, monsterRect))
                {
                    monsterUnit.ApplyHit(15);
                    mOwner.SetAsToDelete();
                    break;
                }
            }
        }

        public override void OnDrawGizmos()
        {
            return;
            base.OnDrawGizmos();
            var center = Pos;
            var bulletRect = new CollisionRect(new Vector2(center.x,center.z),Width,Height,new Vector2(MoveDir.x,MoveDir.z));
            CollisionDebugUtil.DrawRectCorners(bulletRect,Pos.y,0.05f);
            var allMonsters = ZombieBattleMgr.Instance.GetAllMonsterUnits();
            foreach (MonsterUnit monsterUnit in allMonsters)
            {
                var colliderComp = monsterUnit.GetComponent<Collider2DComponent>();
                var monsterPos = monsterUnit.GetComponent<MonsterMoveComponent>().Pos;
                var monsterRect  = new CollisionRect(new Vector2(monsterPos.x,monsterPos.z),colliderComp.Width,colliderComp.Height,new Vector2(0,1));
                CollisionDebugUtil.DrawRectCorners(monsterRect,monsterPos.y,0.05f);
                if (CollisionMathUtil.IsIntersect(bulletRect, monsterRect))
                {
                    Gizmos.DrawCube(monsterPos,new Vector3(0.1f,3,0.1f));
                }
            }
        }
    }
}