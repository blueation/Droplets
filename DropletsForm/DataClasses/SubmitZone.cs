using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Droplets
{
    /// <summary>
    /// SubmitZones are the "inleverzones", they consist of multiple subzones.
    /// </summary>
    class SubmitZone
    {
        private List<SubSubmitZone> subzones;
        public bool Filled = false;

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

        public override bool isCollision(Source source)
        {
            //do we even need to check this collision (do the colours match)?
            if (source.SourceColour == colour)
            { 
                //TODO: VERY SIMPLE implementation, it only checks whether or not the anchor points are inside of the regions.
                //                                  We probably need a more interesting function to handle this.

                //is the source anchored inside of the region
                if ((source.SourceAnchor.X >= x)
                    && (source.SourceAnchor.Y >= y)
                    && (source.SourceAnchor.X <= x + width)
                    && (source.SourceAnchor.Y <= y + height))
                    return true;
                //was the source dragged out to inside of the region
                if ((source.ExtensionAnchor.X >= x)
                    && (source.ExtensionAnchor.Y >= y)
                    && (source.ExtensionAnchor.X <= x + width)
                    && (source.ExtensionAnchor.Y <= y + height))
                    return true;
            }
            return false;
        }

        public override void Draw()
        {
            throw new NotImplementedException(); //TODO: draw square and get the correct Draw() function
        }
    }
}
