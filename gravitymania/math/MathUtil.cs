using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace gravitymania.math
{
	public enum Direction
	{
		None = 0,
		Left,
		Right,
	}

	public static class DirectionMethods
	{
		public static float Sign(this Direction self)
		{
			switch (self)
			{
				case Direction.None:
					return 0.0f;
				case Direction.Left:
					return -1.0f;
				case Direction.Right:
					return 1.0f;
				default:
					throw new Exception("This cannot happen unless you messed up my code.");
			}
		}

		public static Direction Opposite(this Direction self)
		{
			switch (self)
			{
				case Direction.None:
					return Direction.None;
				case Direction.Left:
					return Direction.Right;
				case Direction.Right:
					return Direction.Left;
				default:
					throw new Exception("This cannot happen unless you messed up my code.");
			}
		}
	}

    public static class MathUtil
    {
        public static float Clamp(this float val, float lo, float hi)
        {
            if (val < lo)
            {
                return lo;
            }
            else if (val > hi)
            {
                return hi;
            }
            else
            {
                return val;
            }
        }

        public static Vector2 GetNormalized(this Vector2 self)
        {
            Vector2 result = self;
            result.Normalize();
            return result;
        }

        public static Vector2 GetLeftNorm(this Vector2 self)
        {
            return new Vector2(-self.Y, self.X);
        }

        public static Vector2 GetRightNorm(this Vector2 self)
        {
            return new Vector2(self.Y, -self.X);
        }

		public static int RealRound(this float self)
		{
			if (self >= 0.0f)
			{
				return (int)(self + 0.5f);
			}
			else
			{
				return (int)(self - 0.5f);
			}
		}

		public static Direction GetDirection(this Vector2 self, Vector2 other)
		{
			float dot = Vector2.Dot(self.GetRightNorm().GetNormalized(), other.GetNormalized());

			if (dot == 0.0f)
			{
				return Direction.None;
			}
			else if (dot > 0.0f)
			{
				return Direction.Right;
			}
			else
			{
				return Direction.Left;
			}
		}

        public static bool InRange(this float x, float lo, float hi)
        {
            return lo <= x && hi >= x;
        }
    }
}
