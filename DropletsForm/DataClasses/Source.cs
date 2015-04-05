﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using d2D = System.Drawing.Drawing2D;
using Microsoft.Xna.Framework;

namespace Droplets
{
    /// <summary>
    /// The main object of the game. Contains all necessary data to draw and copy these droplets.
    /// </summary>
    public class Source
    {
        public BlobColour SourceColour;
        public BlobSize SourceSize;
        public Vector2 SourceAnchor;
        public Vector2 ExtensionAnchor;

        public bool Active = true;
        public bool dragged = false;
        //public bool DefaultBehaviour;
        public bool retractThisUpdate = true;

        public Source(BlobColour c, BlobSize s, Vector2 loc)
        {
            SourceColour = c;
            SourceSize = s;
            SourceAnchor = loc;
            ExtensionAnchor = loc;
            //DefaultBehaviour = (SourceColour.ToString() != "White");
        }

        public Source(BlobColour c, BlobSize s, Vector2 loc, bool active, bool dragged)
        {
            SourceColour = c;
            SourceSize = s;
            SourceAnchor = loc;
            ExtensionAnchor = loc;
            Active = active;
            this.dragged = dragged;
            //this.DefaultBehaviour = DefaultBehaviour;
        }

        /// <summary>
        /// checks whether or not a point is on/in a droplet
        /// </summary>
        public bool isIn(float x, float y)
        {
            //Console.WriteLine("totaldistance = {0}", MathHelper.distance(SourceAnchor, x, y) + MathHelper.distance(ExtensionAnchor, x, y));
            //Console.WriteLine("a2 = {0}", MathHelper.calculateEllipseCollisionRadius2(this));
            return MathHelper.distance(SourceAnchor, x, y) + MathHelper.distance(ExtensionAnchor, x, y) <= MathHelper.calculateEllipseCollisionRadius2(this);
        }
        /// <summary>
        /// checks whether or not a vector ends on/in a droplet
        /// </summary>
        public bool isIn(Vector2 v)
        {
            return isIn(v.X, v.Y);
        }
        /// <summary>
        /// returns a tuple equal to the supplied vector, if the vector ends on/in a droplet
        /// </summary>
        public Tuple<float,float> returnIn(Vector2 v)
        {
            if (isIn(v.X, v.Y))
                return new Tuple<float,float>(v.X, v.Y);
            return null;
        }
        /// <summary>
        /// returns whether or not a supplied Droplet collides with this Droplet
        /// </summary>
        /// <param name="Other"></param>
        /// <returns></returns>
        public bool isCollision(Source Other)
        {
            return isIn(Other.SourceAnchor) || isIn(Other.ExtensionAnchor);
        }
        /// <summary>
        /// Returns a point of a supplied Droplet that is on/in a droplet; returns null if there is no collision
        /// </summary>
        public Tuple<float,float> returnCollision(Source Other)
        {
            Tuple<float,float> point = returnIn(Other.SourceAnchor);
            if (point != null)
                return point;
            return returnIn(Other.ExtensionAnchor);
        }

        public void Draw(Graphics g)
        {
            if (!Active)
                return;
            Vector2 mid = MathHelper.midpoint(SourceAnchor, ExtensionAnchor);

            double a, b, c;
            MathHelper.calculateAxis(this, out c, out a, out b);

            float angle = 0;
            if (c > 0)
                angle = MathHelper.angleCalculate(SourceAnchor, ExtensionAnchor);

            //Console.WriteLine("c: {0}\na: {1}\nb: {2}\nx: {3}\nangle: {4}", c, a, b, mid.X, angle);

            d2D.Matrix m = new d2D.Matrix();
            m.RotateAt(angle, new PointF(mid.X, mid.Y));
            g.Transform = m;
            g.FillEllipse(new SolidBrush(SourceColour.screenColor), (int)(mid.X - a), (int)(mid.Y - b), (int)a * 2, (int)b * 2);
            g.ResetTransform();

            //g.FillEllipse(new SolidBrush(SourceColour.screenColor), (int)(mid.X - a), (int)(mid.Y - b), (int)a * 2, (int)b * 2);
            if (dragged)
                g.DrawEllipse(new Pen(SourceColour.screenColor), (int)SourceAnchor.X - SourceSize.getMaxStretch, (int)SourceAnchor.Y - SourceSize.getMaxStretch, SourceSize.getMaxStretch * 2, SourceSize.getMaxStretch * 2);

            Brush complementBrush;
            if (SourceColour.ToString() == "Brown")
                complementBrush = new SolidBrush(System.Drawing.Color.FromArgb(255,153,204,255));
            else
                complementBrush = new SolidBrush(ColourMixer.complement(SourceColour).screenColor);

            g.FillRectangle(complementBrush, (int)SourceAnchor.X - 3, (int)SourceAnchor.Y - 3, 6, 6);
            g.FillRectangle(complementBrush, (int)ExtensionAnchor.X - 3, (int)ExtensionAnchor.Y - 3, 6, 6);
        }
        /// <summary>
        /// Shortens the distance between the two anchors of the droplet
        /// </summary>
        public void Retract()
        {
            float stretch = Convert.ToSingle(MathHelper.distance(SourceAnchor, ExtensionAnchor));
            if (stretch > 0)
            {
                stretch -= SourceSize.getRetractionPerUpdate;
                if (stretch < 0)
                    stretch = 0;
                ExtensionAnchor = ExtensionAnchor - SourceAnchor;
                ExtensionAnchor.Normalize();
                ExtensionAnchor = SourceAnchor + stretch * ExtensionAnchor;
            }
        }
        /// <summary>
        /// Completely retracts the droplet
        /// </summary>
        public void FullRetract()
        {
            ExtensionAnchor = SourceAnchor;
        }
        /// <summary>
        /// Deactivates both collision and drawing of this droplet
        /// </summary>
        public void Deactivate()
        {
            Active = false;
        }
        /// <summary>
        /// Deep copies the droplet
        /// </summary>
        public Source Copy()
        {
            return new Source(new BlobColour().fromString(SourceColour.ToString()), 
                              new BlobSize().fromInt(SourceSize.toInt), 
                              new Vector2(SourceAnchor.X, SourceAnchor.Y),
                              Active,
                              false);
        }

        public override string ToString()
        {
            return SourceAnchor.ToString() + ", " + SourceColour.ToString() + ", " + SourceSize.ToString();
        }
    }
}
