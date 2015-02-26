using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

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
            Tuple<int,int> mid = MathHelper.midpoint(SourceAnchor, ExtensionAnchor);

            double a, b, c;
            MathHelper.calculateAxis(this, out c, out a, out b);

            g.DrawEllipse(new Pen(SourceColour.screenColor), (int)(mid.Item1 - a), (int)(mid.Item2 - b), (int)a * 2, (int)b * 2);

            Console.Write("a:{0}, b:{1}, c:{2}", a, b, c);
        }
    }
}
