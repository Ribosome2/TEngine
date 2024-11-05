using UnityEngine;

namespace GameLogic
{
    public class CollisionDebugUtil
    {
        public static void DrawRectCorners(CollisionRect rect1,float y=1f,float radius=0.2f)
        {
            var prevColor = Gizmos.color;
            Gizmos.color = Color.green;
       
            for (int i = 0; i < 4; i++)
            {
                var pos = rect1.GetCorner(i);
                Gizmos.DrawSphere(new Vector3(pos.x,y,pos.y),radius);
            }
       
            Gizmos.color = prevColor;
        }
    }
}