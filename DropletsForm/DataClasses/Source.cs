using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            //if (MathHelper.distance(SourceAnchor, ExtensionAnchor) < 5) TODO: correctly draw and collide ellipsoids!
            {
                return (MathHelper.distance(SourceAnchor, x, y) < 
            }

            return false;
        }


    }
}
