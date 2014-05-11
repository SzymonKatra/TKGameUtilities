using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace TKGameUtilities
{
    public static class Extensions
    {
        public static float DistanceTo(this Vector2 a, Vector2 b)
        {
            return (float)(Math.Sqrt((b.X - a.X) * (b.X - a.X) + (b.Y - a.Y) * (b.Y - a.Y)));
        }
        public static float AngleTo(this Vector2 a, Vector2 b)
        {
            return GameMath.ReduceAngle(GameMath.ToDegrees((float)(Math.Atan2((a.Y - b.Y), (b.X - a.X)))));
        }
        public static float AngleToRad(this Vector2 a, Vector2 b)
        {
            return GameMath.ReduceAngleRadians((float)(Math.Atan2((a.Y - b.Y), (b.X - a.X))));
        }

        //public static void Rotate(this Vector2 vector, float rotation, Vector2 relative)
        //{
        //    //x' = (x - x2) * cos(rot) - (y - y2) * sin(rot) + x2
        //    //y' = (x - x2) * sin(rot) + (y - y2) * cos(rot) + y2

        //    float sin = (float)Math.Sin(GameMath.ToRadians(rotation));
        //    float cos = (float)Math.Cos(GameMath.ToRadians(rotation));
        //    float px = vector.X - relative.X;
        //    float py = vector.Y - relative.Y;

        //    vector.X = px * cos - py * sin + relative.X;
        //    vector.Y = px * sin + py * cos + relative.Y;
        //}
        

        //public static void RotateRad(this Vector2 vector, float rotationRad, Vector2 relative)
        //{
        //    //x' = (x - x2) * cos(rot) - (y - y2) * sin(rot) + x2
        //    //y' = (x - x2) * sin(rot) + (y - y2) * cos(rot) + y2

        //    float sin = (float)Math.Sin(rotationRad);
        //    float cos = (float)Math.Cos(rotationRad);
        //    float px = vector.X - relative.X;
        //    float py = vector.Y - relative.Y;

        //    vector.X = px * cos - py * sin + relative.X;
        //    vector.Y = px * sin + py * cos + relative.Y;
        //}
        
    }
}
