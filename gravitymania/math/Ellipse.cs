using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace gravitymania.math
{
    public struct Circle
    {
        public Circle(Vector2 position, float radius)
        {
            Position = position;
            Radius = radius;
        }

        public Vector2 Position;
        public float Radius;


    }

    public struct Ellipse
    {
        public Ellipse(Vector2 position, Vector2 size)
        {
            Position = position;
            Size = size;
        }

        public Vector2 Size;
        public Vector2 Position;

        public Vector2 ESpace
        {
            get
            {
                return new Vector2(1.0f / Size.X, 1.0f / Size.Y);
            }
        }

    }
}
