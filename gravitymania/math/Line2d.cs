using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace gravitymania.math
{
    public struct LineEquation
    {
        public float A { get { return Normal.X; } set { Normal.X = value; } }
        public float B { get { return Normal.Y; } set { Normal.Y = value; } }
        public float C;
        public Vector2 Normal;

        public LineEquation(Vector2 point, Vector2 normal)
        {
            Normal = normal;
            normal.Normalize();
            C = 0.0f;
            C = -MultiplyThrough(point);
        }

        public LineEquation(float a, float b, float c)
        {
            Normal = new Vector2(a, b);
            float Length = Normal.Length();
            Normal /= Length;
            C = c / Length;
        }

        public float MultiplyThrough(Vector2 p)
        {
            return A * p.X + B * p.Y + C;
        }

        public Vector2 ClosestPoint(Vector2 p)
        {
            float dist = PointDistance(p);

            return (Normal * dist) + p;
        }

        public float PointDistance(Vector2 p)
        {
			return Math.Abs(SignedDistance(p));
        }

		public float SignedDistance(Vector2 p)
		{
			return MultiplyThrough(p) / Normal.Length();
		}

		public Vector2 RightNorm()
		{
			return Normal.GetRightNorm();
		}

		public Vector2 LeftNorm()
		{
			return Normal.GetLeftNorm();
		}
    }

    public struct LineSegment
    {
        public Vector2 Start;
        public Vector2 End;

        public LineSegment(Vector2 start, Vector2 end)
        {
            Start = start;
            End = end;
        }

        public float LengthSquared()
        {
            return (End - Start).LengthSquared();
        }

        public float Length()
        {
            return (End - Start).Length();
        }

        public LineEquation GetEquation()
        {
            Vector2 diff = End - Start;

            float A = -diff.Y;
            float B = diff.X;

            float C = -(A*End.X + B*End.Y);

            return new LineEquation(A, B, C);
        }

        public Vector2 RightHandNormal()
        {
            Vector2 diff = End - Start;
            return new Vector2(diff.Y, -diff.X);
        }

        public Vector2 LeftHandNormal()
        {
            Vector2 diff = End - Start;
            return new Vector2(-diff.Y, diff.X);
        }

        public bool WithinBoundingBox(Vector2 p, float tolerance = 0.004f)
        {
            return p.X >= Math.Min(Start.X, End.X) - tolerance &&
                p.Y >= Math.Min(Start.Y, End.Y) - tolerance &&
                p.X <= Math.Max(Start.X, End.X) + tolerance &&
                p.Y <= Math.Max(Start.Y, End.Y) + tolerance;
        }
    }
}
