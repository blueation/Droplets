using System;
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
        public bool dragged = false;
        public bool DefaultBehaviour;

        public Source(BlobColour c, BlobSize s, Vector2 loc)
        {
            SourceColour = c;
            SourceSize = s;
            SourceAnchor = loc;
            ExtensionAnchor = loc;
            DefaultBehaviour = (SourceColour.ToString() != "White");
        }

        public Source(BlobColour c, BlobSize s, Vector2 loc, bool active, bool dragged, bool DefaultBehaviour)
        {
            SourceColour = c;
            SourceSize = s;
            SourceAnchor = loc;
            ExtensionAnchor = loc;
            Active = active;
            this.dragged = dragged;
            this.DefaultBehaviour = DefaultBehaviour;
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
        public Tuple<float,float> returnIn(Vector2 v)
        {
            if (isIn(v.X, v.Y))
                return new Tuple<float,float>(v.X, v.Y);
            return null;
        }

        public bool isCollision(Source Other)
        {
            return isIn(Other.SourceAnchor) || isIn(Other.ExtensionAnchor);
        }
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
            d2D.Matrix m = new d2D.Matrix();
            m.RotateAt(angle, new PointF(mid.X, mid.Y));
            g.Transform = m;
            g.FillEllipse(new SolidBrush(SourceColour.screenColor), (int)(mid.X - a), (int)(mid.Y - b), (int)a * 2, (int)b * 2);
            g.ResetTransform();

            //g.FillEllipse(new SolidBrush(SourceColour.screenColor), (int)(mid.X - a), (int)(mid.Y - b), (int)a * 2, (int)b * 2);
            if (dragged)
                g.DrawEllipse(new Pen(SourceColour.screenColor), (int)SourceAnchor.X - SourceSize.getMaxStretch, (int)SourceAnchor.Y - SourceSize.getMaxStretch, SourceSize.getMaxStretch * 2, SourceSize.getMaxStretch * 2);
            g.FillRectangle(new SolidBrush(ColourMixer.complement(SourceColour).screenColor), (int)SourceAnchor.X - 3, (int)SourceAnchor.Y - 3, 6, 6);
            g.FillRectangle(new SolidBrush(ColourMixer.complement(SourceColour).screenColor), (int)ExtensionAnchor.X - 3, (int)ExtensionAnchor.Y - 3, 6, 6);

            //Console.WriteLine("c: {0}\na: {1}\nb: {2}\nx: {3}\nangle: {4}", c, a, b, mid.X, angle);
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

        public void FullRetract()
        {
            ExtensionAnchor = SourceAnchor;
        }

        public void Deactivate()
        {
            Active = false;
        }

        public Source Copy()
        {
            return new Source(new BlobColour().fromString(SourceColour.ToString()), 
                              new BlobSize().fromInt(SourceSize.toInt), 
                              new Vector2(SourceAnchor.X, SourceAnchor.Y),
                              Active,
                              false,
                              DefaultBehaviour);
        }
    }
}
