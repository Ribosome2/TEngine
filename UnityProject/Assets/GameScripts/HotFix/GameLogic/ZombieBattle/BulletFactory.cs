using UnityEngine;

namespace GameLogic
{
    public class BulletFactory
    {
        public static GameUnit CreateDirectionalBullet(Vector3 startPos, Vector3 dir)
        {
            var bulletUnit = new GameUnit();
            var comp=bulletUnit.AddComponent<DirectionalBulletComponent>();
            comp.MoveDir = dir.normalized;
            comp.Pos = startPos;
            bulletUnit.AddComponent<BulletRenderer>();
            return bulletUnit;
        }
    }
}