using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Droplets
{
    /// <summary>
    /// BlobColour is a class used to prevent directly reverencing colors, so the actual color names are hidden behind the simple ones.
    /// </summary>
    public abstract class BlobColour
    {
        /// <summary>
        /// Returns what color should be used to draw this BlobColour
        /// </summary>
        virtual public Color screenColor
        {
            get { return Color.Transparent; }
        }

        public BlobColour fromString(string name)
        {
            if (name == "White")
                return new WhiteColour();
            if (name == "Brown")
                return new BrownColour();
            if (name == "Black")
                return new BlackColour();
            if (name == "Blue")
                return new BlueColour();
            if (name == "Red")
                return new RedColour();
            if (name == "Yellow")
                return new YellowColour();
            if (name == "Purple")
                return new PurpleColour();
            if (name == "Orange")
                return new OrangeColour();
            if (name == "Green")
                return new GreenColour();
            return null;
        }
    }

    public class WhiteColour : BlobColour
    {
        public override Color screenColor
        {
            get { return Color.White; }
        }
    }

    public class BrownColour : BlobColour
    {
        public override Color screenColor
        {
            get { return Color.Brown; }
        }
    }

    public class BlackColour : BlobColour
    {
        public override Color screenColor
        {
            get { return Color.Black; }
        }
    }

    public class BlueColour : BlobColour
    {
        public override Color screenColor
        {
            get { return Color.Blue; }
        }
    }

    public class RedColour : BlobColour
    {
        public override Color screenColor
        {
            get { return Color.Red; }
        }
    }

    public class YellowColour : BlobColour
    {
        public override Color screenColor
        {
            get { return Color.Yellow; }
        }
    }

    public class PurpleColour : BlobColour
    {
        public override Color screenColor
        {
            get { return Color.Purple; }
        }
    }

    public class OrangeColour : BlobColour
    {
        public override Color screenColor
        {
            get { return Color.Orange; }
        }
    }

    public class GreenColour : BlobColour
    {
        public override Color screenColor
        {
            get { return Color.Green; }
        }
    }
}
