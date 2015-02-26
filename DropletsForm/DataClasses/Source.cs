using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Droplets
{
    //TODO: This is currently mostly WIP and only contains any data necessary for reference right now.
    public class Source
    {
        public BlobColour SourceColour;
        public BlobSize SourceSize;
        public Tuple<int, int> SourceAnchor;
        public Tuple<int, int> ExtensionAnchor;

        public Source(BlobColour c, BlobSize s, Tuple<int,int> loc)
        {
            SourceColour = c;
            SourceSize = s;
            SourceAnchor = loc;
            ExtensionAnchor = loc;
        }

        //checks whether or not a point is on/in a droplet
        public bool isIn(int x, int y)
        {
            return MathHelper.distance(SourceAnchor, x, y) + MathHelper.distance(ExtensionAnchor, x, y) <= MathHelper.calculateEllipseCollisionRadius2(this);
        }

        public void Draw(Graphics g)
        {
            Point mid = MathHelper.midpoint(SourceAnchor, ExtensionAnchor);

            double a, b, c;
            MathHelper.calculateAxis(this, out c, out a, out b);

            float angle = MathHelper.angleCalculate(SourceAnchor, ExtensionAnchor);
            Matrix m = new Matrix();
            m.RotateAt(angle, new PointF(mid.X, mid.Y));
            g.Transform = m;
            g.FillEllipse(new SolidBrush(SourceColour.screenColor), (int)(mid.X - a), (int)(mid.Y - b), (int)a * 2, (int)b * 2);
            g.ResetTransform();

        }
    }
}
