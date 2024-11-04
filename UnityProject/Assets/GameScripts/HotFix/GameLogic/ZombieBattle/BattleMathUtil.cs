using UnityEngine;

namespace GameLogic
{
    public class BattleMathUtil
    {
        public static float GetXZSquareMagnitude(Vector3 posA, Vector3 posB)
        {
            return Vector2.SqrMagnitude(new Vector2(posA.x, posA.z)- new Vector2(posB.x, posB.z));
        }
    }
}