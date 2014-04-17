﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace gravitymania.math
{
    public enum CollisionObject
    {
        None = 0,
        Line,
        Ellipse,
        Point,
    }

    public struct CollisionResult
    {
        public CollisionObject Type;
        public float Time;
        public Vector2 Position;
		public Vector2 Normal;
        public bool StartsInside;
        public float PenetrationDistance;
    }

    public static class Collide
    {
        public static bool CollideEllipseWithPoint(Ellipse e, Vector2 velocity, Vector2 point, out CollisionResult result)
        {
            result = new CollisionResult() { Time = 1.0f, Position = point, Normal = e.Position - point, StartsInside = false, Type = CollisionObject.None };
            result.Normal.Normalize();
            
            Vector2 xForm = e.ESpace;
            Vector2 rForm = new Vector2(1.0f / xForm.X, 1.0f / xForm.Y);

	        Vector2 E = xForm * e.Position;
            Vector2 d = xForm * velocity;
            Vector2 f = E - (xForm * point);

            float a = d.LengthSquared();
	        float b = 2.0f * Vector2.Dot(f, d);
            float c = f.LengthSquared() - 1.0f; //radius * radius;

	        float discriminant = b * b - 4 * a * c;
            if (discriminant < 0.0f)
            {
                return false;
            }

	        discriminant = (float) Math.Sqrt(discriminant);
	        float t0 = (-b - discriminant) / (2.0f * a);
	        float t1 = (-b + discriminant) / (2.0f * a);

	        if (t0 >= 0.0f && t0 <= 1.0f) {
		        result.Time = t0;
                result.Type = CollisionObject.Point;
		        return true;
	        }

	        if (t1 >= 0.0f && t1 <= 1.0f) {
                result.Time = t1;
                result.Type = CollisionObject.Point;
		        return true;
	        }

            /*
            if (f.Length() < 1.0f)
            {
                result.StartsInside = true;
                result.Time = 0.0f;
                result.PenetrationDistance = (rForm * ((1.0f - f.Length()) * f)).Length();
                return true;
            }*/
            
	        return false;
        }

        public static bool CollideEllipseWithLine(Ellipse e, Vector2 velocity, LineSegment line, out CollisionResult result)
        {
            result = new CollisionResult() { Time = 1.0f, Position = e.Position, Normal = line.LeftHandNormal(), StartsInside = false, };

            if (velocity.X == 0.0f && velocity.Y == 0.0f)
            {
                return false;
            }

            Vector2 xForm = e.ESpace;
            Vector2 rForm = new Vector2(1.0f / xForm.X, 1.0f / xForm.Y);

            Vector2 xFormedPosition = xForm * e.Position;
            Vector2 xFormedVelocity = xForm * velocity;
            LineSegment xFormedLine = new LineSegment(xForm * line.Start, xForm * line.End);

            LineEquation xFormedEquation = xFormedLine.GetEquation();
            
            if (Vector2.Dot(velocity, xFormedEquation.Normal) > 0.0f)
            {
                return false;
            }

            float t0, t1;
			float distAtStart = xFormedEquation.MultiplyThrough(xFormedPosition); // distPointToLine(circleStart, line1, lineNormal);

            bool embedded = false;

            if (Vector2.Dot(xFormedEquation.Normal, velocity) == 0.0f)
            {
                if (distAtStart >= 1.0f) return false;
                else
                {
                    embedded = true;
                    t0 = 0.0f;
                    t1 = 1.0f;
                }
            }

            float distAtEnd = xFormedEquation.PointDistance(xFormedPosition + xFormedVelocity);

            t0 = (1.0f - distAtStart) / (distAtEnd - distAtStart);
            t1 = (-1.0f - distAtStart) / (distAtEnd - distAtStart);
            if (t0 > t1)
            {
                float temp = t0;
                t0 = t1;
                t1 = temp;
            }
            if (t0 > 1.0f || t1 < 0.0f) return false;
            t0 = MathUtil.Clamp(t0, 0.0f, 1.0f);

            bool hit = false;

            if (!embedded)
            {
                Vector2 finalCenter = xFormedPosition + t0 * xFormedVelocity;
                float hitProportion = Vector2.Dot(finalCenter - xFormedLine.Start, xFormedLine.End - xFormedLine.Start) / xFormedLine.LengthSquared();
                
                if (hitProportion >= 0.0f && hitProportion <= 1.0f)
                {
                    result.Time = t0;
                    result.Position = rForm * (finalCenter - xFormedEquation.Normal);
					result.Normal = (rForm * xFormedEquation.Normal);
                    result.Normal.Normalize();
                    result.Type = CollisionObject.Line;
                    hit = true;
                }
            }

            if (!hit)
            {
                CollisionResult result1, result2;

                if (CollideEllipseWithPoint(e, velocity, line.Start, out result1))
                {
                    result = result1;
                    return true;
                }
                if (CollideEllipseWithPoint(e, velocity, line.End, out result2))
                {
                    if (result2.Time <= result.Time)
                    {
                        result = result2;
                        return true;
                    }
                }
            }

            /*
            if (distAtStart >= 0.0f && distAtStart < 1.0f)
            {
                result.Time = 0.0f;
                result.StartsInside = true;
                result.Normal = rForm * xFormedEquation.Normal;
                result.Normal.Normalize();
                result.Position = rForm * xFormedEquation.ClosestPoint(xFormedPosition);
                result.PenetrationDistance = 1.0f - distAtStart;
                return true;
            }*/

            return hit;
        }
    }
}
