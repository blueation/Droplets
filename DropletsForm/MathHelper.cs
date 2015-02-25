using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Droplets
{
    class MathHelper
    {
        public static double distance(int x, int y, int u, int v)
        {
            return Math.Sqrt(Math.Pow(Math.Abs(x - u), 2) + Math.Pow(Math.Abs(y - v), 2));
        }

        public static double distance(Tuple<int,int> t, int u, int v)
        {
            return distance(t.Item1, t.Item2, u, v);
        }

        public static double distance(Tuple<int, int> t, Tuple<int, int> r)
        {
            return distance(t.Item1, t.Item2, r.Item1, r.Item2);
        }
    }
}
