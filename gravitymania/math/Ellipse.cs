using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace gravitymania.math
{
    public class Ellipse
    {
        public Ellipse(Vector2 position, Vector2 size)
        {
            Position = position;
            Size = size;
        }

        public Vector2 Size;
        public Vector2 Position;

        public Matrix ESpace
        {
            get
            {
                return Matrix.CreateScale(1.0f / Size.X, 1.0f / Size.Y, 1.0f);
            }
        }


    }
}
