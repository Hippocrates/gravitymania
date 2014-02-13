using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace gravitymania.math
{
    public struct AABBox
    {
        public Vector2 Min { get; private set; }
        public Vector2 Max { get; private set; }

        public AABBox(Vector2 min, Vector2 max)
            : this()
        {
            Min = min;
            Max = min;

            AddInternalPoint(min);
            AddInternalPoint(max);
        }

        public static AABBox BuildBox(IEnumerable<Vector2> points)
        {
            AABBox result = new AABBox();

            foreach (var p in points)
            {
                result.AddInternalPoint(p);
            }

            return result;
        }

        public void AddInternalPoint(Vector2 point)
        {
            Min = new Vector2(Math.Min(Min.X, point.X), Math.Min(Min.Y, point.Y));
            Max = new Vector2(Math.Max(Max.X, point.X), Math.Max(Max.Y, point.Y));
        }

        public bool IsContainedIn(Vector2 point)
        {
            return point.X >= Min.X && point.Y >= Min.Y && point.X <= Max.X && point.Y <= Max.Y;
        }

        public bool Overlaps(AABBox other)
        {
            return IsContainedIn(other.Min) || IsContainedIn(other.Max) || other.IsContainedIn(Min) || other.IsContainedIn(Max);
        }
    }

}
