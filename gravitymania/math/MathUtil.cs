using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gravitymania.math
{
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
    }
}
