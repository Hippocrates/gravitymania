﻿using System;
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

        public LineEquation(float a, float b, float c)
        {
            Normal = new Vector2(a, b);
            C = c;
        }

        public float MultiplyThrough(Vector2 p)
        {
            return A * p.X + B * p.Y + C;
        }

        public Vector2 ClosestPoint(Vector2 p)
        {
            float dist = PointDistance(p);

            return (-Normal * dist) + p;
        }

        public float PointDistance(Vector2 p)
        {
            return Math.Abs(MultiplyThrough(p)) / Normal.Length();
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
