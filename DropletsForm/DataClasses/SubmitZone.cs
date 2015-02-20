using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Droplets
{
    /// <summary>
    /// SubmitZones are the "inleverzones", they consist of multiple subzones.
    /// </summary>
    class SubmitZone
    {
        private List<SubSubmitZone> subzones;

        public void AddZone(SubSubmitZone z)
        {
            subzones.Add(z);
        }

        public void Draw() //TODO: Get the correct Draw() function!
        {
            foreach (SubSubmitZone z in subzones)
            {
                z.Draw();
            }
        }

        /// <summary>
        /// Used to detect whether or not a Droplet touches the SubmitZone
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public bool isCollision(Source source)
        {
            bool result = false;
            for (int i = 0; i < subzones.Count() && !result; i++)
            {
                result = subzones[i].isCollision(source);
            }
            return result;
        }
    }

    abstract class SubSubmitZone
    {
        abstract public void Draw();
        abstract public Tuple<int,int> Collide(Source source);
        abstract public bool isCollision(Source source);
    }

    class SquareSubmitZone : SubSubmitZone
    {
        int x, y;
        int width, height;
        BlobColour colour;

        public SquareSubmitZone(int x, int y, int width, int height, BlobColour colour)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.colour = colour;
        }

        public override Tuple<int,int> Collide(Source source)
        {
            //do we even need to check this collision (do the colours match)?
            if (source.SourceColour == colour)
            { 
                //TODO: VERY SIMPLE implementation, it only checks whether or not the anchor points are inside of the regions.
                //                                  We probably need a more interesting function to handle this.

                //is the source anchored inside of the region
                if (   (source.SourceAnchor.Item1 >= x) 
                    && (source.SourceAnchor.Item2 >= y)
                    && (source.SourceAnchor.Item1 <= x + width) 
                    && (source.SourceAnchor.Item2 <= y + height) )
                    return source.SourceAnchor;
                //was the source dragged out to inside of the region
                if (   (source.ExtensionAnchor.Item1 >= x)
                    && (source.ExtensionAnchor.Item2 >= y)
                    && (source.ExtensionAnchor.Item1 <= x + width)
                    && (source.ExtensionAnchor.Item2 <= y + height))
                    return source.ExtensionAnchor;
            }
            return null;
        }

        public override bool isCollision(Source source)
        {
            if (Collide(source) == null)
                return false;
            return true;
        }

        public override void Draw()
        {
            throw new NotImplementedException(); //TODO: draw square and get the correct Draw() function
        }
    }
}
