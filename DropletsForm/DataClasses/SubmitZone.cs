using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Microsoft.Xna.Framework;

namespace Droplets
{
    /// <summary>
    /// SubmitZones are the "inleverzones", they consist of multiple subzones.
    /// </summary>
    class SubmitZone
    {
        private List<SubSubmitZone> subzones = new List<SubSubmitZone>();
        public bool Filled = false;
        public BlobColour colour;

        public void AddZone(SubSubmitZone z)
        {
            subzones.Add(z);
        }

        public void Draw(Graphics g) //TODO: Get the correct Draw() function!
        {
            foreach (SubSubmitZone z in subzones)
            {
                z.Draw(g);
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
        abstract public void Draw(Graphics g);
        abstract public bool isCollision(Source source);
    }

    class SquareSubmitZone : SubSubmitZone
    {
        int x, y;
        int width, height;
        SubmitZone parent;

        public SquareSubmitZone(int x, int y, int width, int height, SubmitZone parent)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.parent = parent;
        }

        public override bool isCollision(Source source)
        {
            //do we even need to check this collision (do the colours match)?
            if (source.SourceColour.ToString() == parent.colour.ToString())
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

        public override void Draw(Graphics g)
        {
            g.FillRectangle(new SolidBrush(parent.colour.screenColor), x, y, width, height);
        }
    }

    class CircleSubmitZone : SubSubmitZone
    {
        Vector2 mid;
        int x, y;
        int diameter;
        int radius;
        SubmitZone parent;

        public CircleSubmitZone(int x, int y, int diameter, SubmitZone parent)
        {
            this.radius = diameter / 2;
            this.mid = new Vector2(x + radius, y + radius);
            this.x = x;
            this.y = y;
            this.diameter = diameter;
            this.parent = parent;
        }

        public override bool isCollision(Source source)
        {
            //do we even need to check this collision (do the colours match)?
            if (source.SourceColour.ToString() == parent.colour.ToString())
            {
                //is the source anchored inside of the region
                if (MathHelper.distance(source.SourceAnchor, mid) <= radius)
                    return true;
                //was the source dragged out to inside of the region
                if (MathHelper.distance(source.ExtensionAnchor, mid) <= radius)
                    return true;
            }
            return false;
        }

        public override void Draw(Graphics g)
        {
            g.FillEllipse(new SolidBrush(parent.colour.screenColor), x, y, diameter, diameter);
        }
    }
}
