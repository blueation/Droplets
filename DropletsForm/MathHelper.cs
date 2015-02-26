using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Droplets
{
    class MathHelper
    {
        public static void calculateAxis(Source s, out double c, out double a, out double b)
        {
            c = distance(s.SourceAnchor, s.ExtensionAnchor) / 2;
            
            double e = 1 - (1 / (c + 1)); //eccentricity, e = 0 at c = 0; e = 1 at c = inf
                                          //TODO: other way of calculating an e; in practice this gives a = c+1...
            double e2 = Math.Pow(e, 2);

            if (c != 0)
            {
                //a = c / e;
                //b = Math.Sqrt(Math.Pow(a, 2) - Math.Pow(c, 2));

                a = s.SourceSize.getRadius / Math.Pow(1 - e2, 0.25);
                b = Math.Pow(s.SourceSize.getRadius, 2) / a;
            }
            else
            {
                a = s.SourceSize.getRadius;
                b = a;
            }
        }

        /// <summary>
        /// calculates the summed distance a point must not be greater than, to still be inside of the ellips
        /// </summary>
        /// <returns></returns>
        public static double calculateEllipseCollisionRadius2(Source s)
        {
            double a, b, c;
            calculateAxis(s, out c, out a, out b);
            return 2 * a;
        }

        /// <summary>
        /// calculates the distance between two points
        /// </summary>
        /// <returns>the distance between two points</returns>
#region distance overloads
        public static double distance(int x, int y, int u, int v)
        {
            return Math.Sqrt(Math.Pow(x - u, 2) + Math.Pow(y - v, 2));
        }

        public static double distance(Tuple<int,int> t, int u, int v)
        {
            return distance(t.Item1, t.Item2, u, v);
        }

        public static double distance(Tuple<int, int> t, Tuple<int, int> r)
        {
            return distance(t.Item1, t.Item2, r.Item1, r.Item2);
        }
#endregion

        /// <summary>
        /// calculates the midpoint between two points
        /// </summary>
        /// <returns>the midpoint between two points</returns>
        #region midpoint overloads
        public static Point midpoint(int x, int y, int u, int v)
        {
            return new Point(x + (u - x)/2, y + (v - y)/2);
        }

        public static Point midpoint(Tuple<int, int> t, int u, int v)
        {
            return midpoint(t.Item1, t.Item2, u, v);
        }

        public static Point midpoint(Tuple<int, int> t, Tuple<int, int> r)
        {
            return midpoint(t.Item1, t.Item2, r.Item1, r.Item2);
        }
        #endregion

        /// <summary>
        /// calculates the angle between two points
        /// </summary>
        /// <returns>the angle between two points</returns>
        #region midpoint overloads
        public static float angleCalculate(int x, int y, int u, int v)
        {
            return Convert.ToSingle(Math.Atan2(v - y, u - x) * (180 / Math.PI));
        }

        public static float angleCalculate(Tuple<int, int> t, int u, int v)
        {
            return angleCalculate(t.Item1, t.Item2, u, v);
        }

        public static float angleCalculate(Tuple<int, int> t, Tuple<int, int> r)
        {
            return angleCalculate(t.Item1, t.Item2, r.Item1, r.Item2);
        }
        #endregion
    }
}
