using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using gravitymania.math;

namespace gravitymania.collision
{
    public enum CollisionGeometry
    {
        None = 0,
        Line,
        Ellipse,
        Point,
    }

    public struct CollisionResult
    {
        public CollisionGeometry Type;
        public float Time;
        public Vector2 Position;
		public Vector2 Normal;
		public bool Hit;
    }

	public struct OverlapResult
	{
		public CollisionGeometry Type;
		public float OverlappingDistance;
		public bool Overlapping;
		public Vector2 EjectionNormal;
		public Vector2 EjectionPoint;
	}

    public static class Collide
    {
		// This is a special method for checking if you are already inside the point, it should be used as a final backup 
		// check at the beginning of the frame to make sure you avoid starting out in a bad state.
		public static bool CheckOverlapEllipseWithPoint(Ellipse e, Vector2 point, out OverlapResult result)
		{
			result = new OverlapResult() { Overlapping = false, };

			Vector2 xForm = e.ESpace;
			Vector2 rForm = e.Size;

			Vector2 xFormedPosition = xForm * e.Position;
			Vector2 xFormedPoint = (xForm * point);
			Vector2 xFormedOffset = xFormedPosition - xFormedPoint;

			float offsetLengthSq = xFormedOffset.LengthSquared();

			if (offsetLengthSq < 1.0f)
			{
				Vector2 circleEdge = (Math.Abs(offsetLengthSq) < 0.00001f) ? new Vector2(0.0f, 1.0f) : -xFormedOffset.GetNormalized();

				result.Overlapping = true;
				result.OverlappingDistance = ((circleEdge + xFormedOffset) * rForm).Length();
				result.EjectionNormal = (rForm * xFormedOffset).GetNormalized();
				result.EjectionPoint = point;
				result.Type = CollisionGeometry.Point;

				return true;
			}

			return false;
		}

		// This is a special method for checking if you are already inside the line, it should be used as a final backup 
		// check at the beginning of the frame to make sure you avoid starting out in a bad state.
		public static bool CheckOverlapEllipseWithLine(Ellipse e, LineSegment line, out OverlapResult result)
		{
			Vector2 xForm = e.ESpace;
			Vector2 rForm = e.Size;

			Vector2 xFormedPosition = xForm * e.Position;
			LineSegment xFormedLine = new LineSegment(xForm * line.Start, xForm * line.End);

			LineEquation xFormedEquation = xFormedLine.GetEquation();

			result = new OverlapResult() { Overlapping = false, };

			float distAtStart = xFormedEquation.SignedDistance(xFormedPosition); // distPointToLine(circleStart, line1, lineNormal);

			if (Math.Abs(distAtStart) < 1.0f)
			{
				Vector2 closestPoint = xFormedEquation.ClosestPoint(xFormedPosition);
				if (xFormedLine.WithinBoundingBox(closestPoint))
				{
					result.Type = CollisionGeometry.Line;
					result.Overlapping = true;
					Vector2 circleEdge = -xFormedEquation.Normal + xFormedPosition;
					result.OverlappingDistance = ((circleEdge - closestPoint) * rForm).Length();
					result.EjectionNormal = xFormedEquation.Normal;
					result.EjectionPoint = closestPoint * rForm;
					return true;
				}
			    // else check overlap/collision with endpoints (note that you can exit after this point I think, since if you started
			    // at this distance, don't cross the enpoints, and aren't initially touching the line, you also cannot be touching the line
			    // at any point in the interim
				else
				{
					OverlapResult result1, result2;
					bool hit = false;

					if (CheckOverlapEllipseWithPoint(e, line.Start, out result1))
					{
						result = result1;
						hit = true;
					}
					if (CheckOverlapEllipseWithPoint(e, line.End, out result2))
					{
						if (!result.Overlapping || result2.OverlappingDistance < result.OverlappingDistance)
						{
							result = result2;
							hit = true;
						}
					}

					if (hit)
					{
						return true;
					}
				}
			}

			return false;
		}

        public static bool CollideEllipseWithPoint(Ellipse e, Vector2 velocity, Vector2 point, out CollisionResult result)
        {
            result = new CollisionResult() { Time = 1.0f, Position = point, Hit = false, Type = CollisionGeometry.None };

            Vector2 xForm = e.ESpace;
			Vector2 rForm = e.Size;

	        Vector2 xFormedPosition = xForm * e.Position;
            Vector2 xFormedVelocity = xForm * velocity;
			Vector2 xFormedPoint = (xForm * point);
            Vector2 xFormedOffset = xFormedPosition - xFormedPoint;

			// if the direction is _away_ from the point, don't collide
			if (Vector2.Dot(xFormedVelocity, xFormedOffset) > 0.0f)
			{
				return false;
			}

            float a = xFormedVelocity.LengthSquared();
	        float b = 2.0f * Vector2.Dot(xFormedOffset, xFormedVelocity);
            float c = xFormedOffset.LengthSquared() - 1.0f; //radius * radius;

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
				Vector2 hitPosition = xFormedPosition + (xFormedVelocity * t0);
				result.Normal = (xForm * (hitPosition - xFormedPoint)).GetNormalized();
                result.Type = CollisionGeometry.Point;
                result.Hit = true;
		        return true;
	        }

	        if (t1 >= 0.0f && t1 <= 1.0f) {
                result.Time = t1;
				Vector2 hitPosition = xFormedPosition + (xFormedVelocity * t0);
				result.Normal = (xForm * (hitPosition - point)).GetNormalized();
                result.Type = CollisionGeometry.Point;
                result.Hit = true;
		        return true;
	        }

	        return false;
        }

        public static bool CollideEllipseWithLine(Ellipse e, Vector2 velocity, LineSegment line, out CollisionResult result)
        {
            result = new CollisionResult() { Time = 1.0f, Position = e.Position, Normal = new Vector2(0.0f, 0.0f), Hit = false, };

            if (velocity.X == 0.0f && velocity.Y == 0.0f)
            {
                return false;
            }

            Vector2 xForm = e.ESpace;
			Vector2 rForm = e.Size;

            Vector2 xFormedPosition = xForm * e.Position;
            Vector2 xFormedVelocity = xForm * velocity;
            LineSegment xFormedLine = new LineSegment(xForm * line.Start, xForm * line.End);

            LineEquation xFormedEquation = xFormedLine.GetEquation();

			if (Vector2.Dot(xFormedVelocity, xFormedEquation.Normal) > 0.0f)
			{
				return false;
			}

            bool embedded = false;
            float t0, t1;
			float distAtStart = xFormedEquation.SignedDistance(xFormedPosition); // distPointToLine(circleStart, line1, lineNormal);

			if (Vector2.Dot(xFormedEquation.Normal, xFormedVelocity) == 0.0f)
            {
                if (distAtStart >= 1.0f) return false;
                else
                {
                    embedded = true;
                    t0 = 0.0f;
                    t1 = 1.0f;
                }
            }

			float distAtEnd = xFormedEquation.SignedDistance(xFormedPosition + xFormedVelocity);

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

            if (!embedded)
            {
                Vector2 finalCenter = xFormedPosition + t0 * xFormedVelocity;
                float hitProportion = Vector2.Dot(finalCenter - xFormedLine.Start, xFormedLine.End - xFormedLine.Start) / xFormedLine.LengthSquared();
                
                if (hitProportion >= 0.0f && hitProportion <= 1.0f)
                {
                    result.Time = t0;
                    result.Position = rForm * (finalCenter - xFormedEquation.Normal);
					result.Normal = line.LeftHandNormal().GetNormalized();
                    result.Type = CollisionGeometry.Line;
					result.Hit = true;
					return true;
                }
            }

			bool hit = false;

            CollisionResult result1, result2;
                
            if (CollideEllipseWithPoint(e, velocity, line.Start, out result1))
            {
                result = result1;
                hit = true;
            }
            if (CollideEllipseWithPoint(e, velocity, line.End, out result2))
            {
                if (!result.Hit || result2.Time <= result.Time)
                {
                    result = result2;
                    hit = true;
                }
            }

            return hit;
        }
    }
}
