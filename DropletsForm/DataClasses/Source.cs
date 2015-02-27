﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using d2D = System.Drawing.Drawing2D;
using Microsoft.Xna.Framework;

namespace Droplets
{
    public class Source
    {
        public BlobColour SourceColour;
        public BlobSize SourceSize;
        public Vector2 SourceAnchor;
        public Vector2 ExtensionAnchor;

        public bool Active = true;
        public bool DefaultBehaviour;

        public Source(BlobColour c, BlobSize s, Vector2 loc)
        {
            SourceColour = c;
            SourceSize = s;
            SourceAnchor = loc;
            ExtensionAnchor = loc;
            DefaultBehaviour = (SourceColour.ToString() != "White");
        }

        //checks whether or not a point is on/in a droplet
        public bool isIn(float x, float y)
        {
            //Console.WriteLine(MathHelper.distance(SourceAnchor, x, y) + MathHelper.distance(ExtensionAnchor, x, y));
            //Console.WriteLine(MathHelper.calculateEllipseCollisionRadius2(this));
            return MathHelper.distance(SourceAnchor, x, y) + MathHelper.distance(ExtensionAnchor, x, y) <= MathHelper.calculateEllipseCollisionRadius2(this);
        }
        public bool isIn(Vector2 v)
        {
            return isIn(v.X, v.Y);
        }

        public bool isCollision(Source Other)
        {
            return isIn(Other.SourceAnchor) || isIn(Other.ExtensionAnchor);
        }

        public void Draw(Graphics g)
        {
            Vector2 mid = MathHelper.midpoint(SourceAnchor, ExtensionAnchor);

            double a, b, c;
            MathHelper.calculateAxis(this, out c, out a, out b);

            float angle = MathHelper.angleCalculate(SourceAnchor, ExtensionAnchor);
            d2D.Matrix m = new d2D.Matrix();
            m.RotateAt(angle, new PointF(mid.X, mid.Y));
            g.Transform = m;
            g.FillEllipse(new SolidBrush(SourceColour.screenColor), (int)(mid.X - a), (int)(mid.Y - b), (int)a * 2, (int)b * 2);
            g.ResetTransform();
        }

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

        public void Deactivate()
        {
            Active = false;
        }
    }
}
