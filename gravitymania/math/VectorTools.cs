using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace gravitymania.math
{
    public static class VectorTools
    {
        public static Vector2 GetCentroid(IEnumerable<Vector2> vertices)
        {
            Vector2 output = Vector2.Zero;

            int count = 0;

            foreach (Vector2 v in vertices)
            {
                output += v;
                ++count;
            }

            if (count > 0)
            {
                output.X /= count;
                output.Y /= count;
            }

            return output;
        }
    }
}
