using System;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 2D的碰撞检测结构体
    /// </summary>
    public struct CollisionRect
    {

        /// <summary>
        /// 通过朝向构建
        /// </summary>
        /// <param name="center"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="forward">这是向前的向量，主要如果是3D的方向要注意传什么轴，如果是XZ轴对比就传XZ轴</param>
        public CollisionRect(Vector2 center, float width, float height, Vector2 forward)
        {
            float angle=  -(float)Math.Atan2(forward.x, forward.y);
            
            this.Center = center;
            this.Width = width;
            this.Height = height;
            this.RotateAngle = angle;
            
            // 计算矩形的四个顶点
            float halfWidth = width / 2f;
            float halfHeight = height / 2f;

            float sin = (float)Math.Sin(angle);
            float cos = (float)Math.Cos(angle);

            Vector2 corner1 = new Vector2(-halfWidth, -halfHeight);
            Vector2 corner2 = new Vector2(halfWidth, -halfHeight);
            Vector2 corner3 = new Vector2(halfWidth, halfHeight);
            Vector2 corner4 = new Vector2(-halfWidth, halfHeight);

            corner1 = new Vector2((corner1.x* cos) - (corner1.y * sin), (corner1.x * sin) + (corner1.y * cos));
            corner2 = new Vector2((corner2.x* cos) - (corner2.y * sin), (corner2.x * sin) + (corner2.y * cos));
            corner3 = new Vector2((corner3.x* cos) - (corner3.y * sin), (corner3.x * sin) + (corner3.y * cos));
            corner4 = new Vector2((corner4.x* cos) - (corner4.y * sin), (corner4.x * sin) + (corner4.y * cos));

            this.Corner1 = corner1 + center;
            this.Corner2 = corner2 + center;
            this.Corner3 = corner3 + center;
            this.Corner4 = corner4 + center;
        }
        /// <summary>
        /// angle 是旋转的角度，通过
        /// </summary>
        /// <param name="center"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="angle"></param>
        public CollisionRect(Vector2 center, float width, float height, float angle)
        {
            this.Center = center;
            this.Width = width;
            this.Height = height;
            this.RotateAngle = angle;
            // 计算矩形的四个顶点
            float halfWidth = width / 2f;
            float halfHeight = height / 2f;

            float sin = (float)Math.Sin(angle);
            float cos = (float)Math.Cos(angle);

            Vector2 corner1 = new Vector2(-halfWidth, -halfHeight);
            Vector2 corner2 = new Vector2(halfWidth, -halfHeight);
            Vector2 corner3 = new Vector2(halfWidth, halfHeight);
            Vector2 corner4 = new Vector2(-halfWidth, halfHeight);

            corner1 = new Vector2((corner1.x* cos) - (corner1.y * sin), (corner1.x * sin) + (corner1.y * cos));
            corner2 = new Vector2((corner2.x* cos) - (corner2.y * sin), (corner2.x * sin) + (corner2.y * cos));
            corner3 = new Vector2((corner3.x* cos) - (corner3.y * sin), (corner3.x * sin) + (corner3.y * cos));
            corner4 = new Vector2((corner4.x* cos) - (corner4.y * sin), (corner4.x * sin) + (corner4.y * cos));

            this.Corner1 = corner1 + center;
            this.Corner2 = corner2 + center;
            this.Corner3 = corner3 + center;
            this.Corner4 = corner4 + center;
        }
        public Vector2 Center;
        public float Width;
        public float Height;
        /// 在平面上旋转的弧度
        /// </summary>
        public float RotateAngle; //whe rotate angle of the rect
        public  Vector2 GetCorner( int index)
        {
            switch (index)
            {
                case 0:
                    return Corner1;
                case 1:
                    return Corner2;
                case 2:
                    return Corner3;
                case 3:
                    return Corner4;
                default:
                    throw new ArgumentOutOfRangeException(nameof(index));
            }
        }
        
        /// <summary>
        /// 在平面上旋转的弧度
        /// </summary>
        public Vector2 Corner1; 
        public Vector2 Corner2; 
        public Vector2 Corner3; 
        public Vector2 Corner4; 
    }
}