using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gravitymania.math
{
    public static class NumTools
    {
        public static bool SameSign(float x, float y)
        {
            return (x * y) >= 0.0f;
        }

        public static bool DifferentSign(float x, float y)
        {
            return (x * y) < 0.0f;
        }

        public static int Min(int first, int second, params int[] rest)
        {
            int min = Math.Min(first, second);

            foreach (int i in rest)
            {
                min = Math.Min(min, i);
            }

            return min;
        }

        public static bool FEquals(this float x, float y, float epsilon = 0.00001f)
        {
            return Math.Abs(x - y) < epsilon;
        }

        public static Int16 FloatToInt16(float input)
        {
            return (Int16)(input * Int16.MaxValue);
        }

        public static Int16 DoubleToInt16(double input)
        {
            return (Int16)(input * Int16.MaxValue);
        }

        public static double Int16ToDouble(Int16 input)
        {
            return (double)input / Int16.MaxValue;
        }

        public static float Int16ToFloat(Int16 input)
        {
            return (float)input / Int16.MaxValue;
        }

        public static int Clamp(int value, int min, int max)
        {
            return Math.Max(min, Math.Min(max, value));
        }

        public static float Clamp(float value, float min, float max)
        {
            return Math.Max(min, Math.Min(max, value));
        }
    }
}
