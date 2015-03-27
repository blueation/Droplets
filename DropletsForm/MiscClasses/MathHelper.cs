using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Microsoft.Xna.Framework;

namespace Droplets
{ 
    class MathHelper
    {
        public static void calculateAxis(Source s, out double c, out double a, out double b)
        {
            c = distance(s.SourceAnchor, s.ExtensionAnchor) / 2;
            
            if (c != 0)
            {
                double r = s.SourceSize.getRadius;
                double c2 = - c * c;
                double c4 = c2 * c2;
                double r4 = - r * r * r * r;

                double D = c4 - 4 * r4;
                double B2D = -c2 + Math.Pow(D, 0.5);
                double B2DA = B2D / 2;
                a = Math.Pow(B2DA, 0.5);
                b = Math.Pow(a*a - c*c, 0.5);
            }
            else
            {
                a = s.SourceSize.getRadius;
                b = a;
            }
        }

        /// <summary>
        /// calculates the summed distance a point must not be greater than, to still be inside of the ellips
        /// Doesn't actually work as intended, a lot of space outside of the ellips seems to fall under this... (false positives)
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
        public static double distance(float x, float y, float u, float v)
        {
            return Math.Sqrt((x-u)*(x-u) + (y-v)*(y-v));
        }

        public static double distance(Vector2 v1, float x, float y)
        {
            return distance(v1.X, v1.Y, x, y);
        }

        public static double distance(Vector2 v1, Vector2 v2)
        {
            return distance(v1.X, v1.Y, v2.X, v2.Y);
        }
#endregion

        /// <summary>
        /// calculates the midpoint between two points
        /// </summary>
        /// <returns>the midpoint between two points</returns>
        #region midpoint overloads
        public static Vector2 midpoint(float x, float y, float u, float v)
        {
            return new Vector2(x + (u - x)/2, y + (v - y)/2);
        }

        public static Vector2 midpoint(Vector2 v1, float x, float y)
        {
            return midpoint(v1.X, v1.X, x, y);
        }

        public static Vector2 midpoint(Vector2 v1, Vector2 v2)
        {
            return midpoint(v1.X, v1.Y, v2.X, v2.Y);
        }
        #endregion

        /// <summary>
        /// calculates the angle between a line defined by two points and a horizontal through the first point
        /// </summary>
        /// <returns></returns>
        #region angleCalculate overloads
        public static float angleCalculate(float x, float y, float u, float v)
        {
            return Convert.ToSingle(Math.Atan2(v - y, u - x) * (180 / Math.PI));
        }

        public static float angleCalculate(Vector2 v1, float x, float y)
        {
            return angleCalculate(v1.X, v1.Y, x, y);
        }

        public static float angleCalculate(Vector2 v1, Vector2 v2)
        {
            return angleCalculate(v1.X, v1.Y, v2.X, v2.Y);
        }
        #endregion

        public static Vector2 toVector(Tuple<float,float> tup)
        {
            return new Vector2(tup.Item1, tup.Item2);
        }
    }
}
