using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace TKGameUtilities.Graphics
{
    public struct Camera2D
    {
        public Vector2 Position;
        public float Rotation;
        public Vector2 Origin;
        public Vector2 Scale;
        public Rectangle Viewport;

        public Matrix4 GetTransform()
        {
            //thanks to Leri from stackoverflow.com - http://stackoverflow.com/questions/712296/xna-2d-camera-engine-that-follows-sprite

            //return Matrix4.Identity *
            //       Matrix4.CreateTranslation(-Position.X, -Position.Y, 0) *
            //       Matrix4.CreateRotationZ(Rotation) *
            //       Matrix4.CreateTranslation(Origin.X, Origin.Y, 0) *
            //       Matrix4.CreateScale(new Vector3(Scale.X, Scale.Y, 1));
            return Matrix4.Identity *
                   Matrix4.CreateTranslation(-Position.X, -Position.Y, 0) *
                   Matrix4.CreateScale(new Vector3(Scale.X, Scale.Y, 1)) *
                   Matrix4.CreateRotationZ(GameMath.ToRadians(Rotation)) *
                   Matrix4.CreateTranslation(Origin.X, Origin.Y, 0);
            //return Matrix4.Identity *
            //       Matrix4.CreateRotationZ(GameMath.ToRadians(Rotation)) *
            //       Matrix4.CreateScale(new Vector3(Scale.X, Scale.Y, 1)) *
            //       Matrix4.CreateTranslation(-Position.X, -Position.Y, 0) *
            //       Matrix4.CreateTranslation(Origin.X, Origin.Y, 0);
        }
    }
}
