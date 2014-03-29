using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace TKGameUtilities
{
    public struct RayCastResult
    {
        public bool Collides;
        public Vector2 CollisionPoint;
        public float RayLength;
    }
}
