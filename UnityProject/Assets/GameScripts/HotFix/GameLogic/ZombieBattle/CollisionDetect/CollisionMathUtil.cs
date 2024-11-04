using System;
using UnityEngine;

namespace GameLogic
{
   
    public static class CollisionMathUtil
    {
        /// <summary>
        /// In this example, we create a Vector2 called forward that represents the forward direction of an object.
        /// We then use the Atan2 function to calculate the angle between the forward vector and the positive X-axis.
        /// The result is in radians, so you may need to convert it to degrees if necessary.
        /// Note that the Atan2 function returns an angle in the range of -π to π,
        /// so you may need to adjust the angle if you want it to be in a different range.
        /// For example, if you want the angle to be in the range of 0 to 2π, you can add 2π to the angle if it's negative:
        /// </summary>
        /// <param name="forward"></param>
        /// <returns></returns>
        public static float ForwardDirectionToAngle(Vector2 forward)
        {
            float rotateAngle = -(float)Math.Atan2(forward.x, forward.y);
            return rotateAngle;
        }
        
        public static float ForwardDirectionToAngle(Vector3 forward)
        {
            float rotateAngle = -(float)Math.Atan2(forward.x, forward.z);
            return rotateAngle;
        }
        
        public static bool IsIntersect(CollisionRect rectA, CollisionRect rectB)
        {
            
            // 检测矩形A是否包含矩形B
            if (IsRectangleContains(rectA, rectB))
            {
                return true;
            }

            // 检测矩形B是否包含矩形A
            if (IsRectangleContains(rectB, rectA))
            {
                return true;
            }
            
            // 对于矩形A的每一条边
            for (int i = 0; i < 4; i++)
            {
                // 获取当前边的起点和终点
                Vector2 p1 = rectA.GetCorner( i);
                Vector2 p2 = rectA.GetCorner( (i + 1) % 4);

                // 对于矩形B的每一条边
                for (int j = 0; j < 4; j++)
                {
                    // 获取当前边的起点和终点
                    Vector2 p3 = rectB.GetCorner( j);
                    Vector2 p4 = rectB.GetCorner( (j + 1) % 4);

                    // 检测两条线段是否相交
                    if (LinesIntersect(p1, p2, p3, p4))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        
        private static bool LinesIntersect(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
        {
            float denominator = ((p4.y - p3.y) * (p2.x - p1.x)) - ((p4.x - p3.x) * (p2.y - p1.y));

            if (denominator == 0)
            {
                return false;
            }

            float ua = (((p4.x - p3.x) * (p1.y - p3.y)) - ((p4.y - p3.y) * (p1.x - p3.x))) / denominator;
            float ub = (((p2.x - p1.x) * (p1.y - p3.y)) - ((p2.y - p1.y) * (p1.x - p3.x))) / denominator;

            if (ua < 0 || ua > 1 || ub < 0 || ub > 1)
            {
                return false;
            }

            return true;
        }
        
        private static bool IsRectangleContains(CollisionRect rectA, CollisionRect rectB)
        {
            // 检测矩形B的四个顶点是否都在矩形A内部
            for (int i = 0; i < 4; i++)
            {
                Vector2 corner = rectB.GetCorner( i);

                if (!IsPointInRectangle(corner, rectA))
                {
                    return false;
                }
            }

            return true;
        }
        private static Vector2 RotateVector(Vector2 v, float angle)
        {
            float sin = (float)Math.Sin(angle);
            float cos = (float)Math.Cos(angle);

            float x = (cos * v.x) - (sin * v.y);
            float y = (sin * v.x) + (cos * v.y);

            return new Vector2(x, y);
        }
        private static bool IsPointInRectangle(Vector2 point, CollisionRect rect)
        {
            // 将点转换到矩形的本地坐标系中
            Vector2 localPoint = point - rect.Center;
            localPoint = RotateVector(localPoint, -rect.RotateAngle);

            // 检测点是否在矩形的范围内
            float halfWidth = rect.Width / 2f;
            float halfHeight = rect.Height / 2f;

            if (localPoint.x < -halfWidth || localPoint.x > halfWidth || localPoint.y < -halfHeight || localPoint.y > halfHeight)
            {
                return false;
            }

            return true;
        }


    }
}