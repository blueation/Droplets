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
    public class BlobColour
    {
        /// <summary>
        /// Returns what color should be used to draw this BlobColour
        /// </summary>
        virtual public Color screenColor
        {
            get { return Color.Transparent; }
        }

        virtual public Color secondColor
        {
            get { return Color.Transparent; }
        }

        virtual public int mixerHelper
        {
            get { return 0; }
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

        public override Color secondColor
        {
            get { return Color.FromArgb(255, 204, 204, 204); }
        }

        public override string ToString()
        {
            return "White";
        }

        public override int mixerHelper
        {
            get { return 1; }
        }
    }

    public class BrownColour : BlobColour
    {
        public override Color screenColor
        {
            get { return Color.FromArgb(255, 102, 51, 0); }
        }

        public override Color secondColor
        {
            get { return Color.FromArgb(255, 125, 81, 38); }
        }

        public override string ToString()
        {
            return "Brown";
        }

        public override int mixerHelper
        {
            get { return 10; }
        }
    }

    public class BlackColour : BlobColour
    {
        public override Color screenColor
        {
            get { return Color.Black; }
        }

        public override Color secondColor
        {
            get { return Color.FromArgb(255,51,51,51); }
        }

        public override string ToString()
        {
            return "Black";
        }

        public override int mixerHelper
        {
            get { return 100; }
        }
    }

    public class BlueColour : BlobColour
    {
        public override Color screenColor
        {
            get { return Color.Blue; }
        }

        public override Color secondColor
        {
            get { return Color.FromArgb(255, 51, 51, 255); }
        }

        public override string ToString()
        {
            return "Blue";
        }

        public override int mixerHelper
        {
            get { return 1000; }
        }
    }

    public class RedColour : BlobColour
    {
        public override Color screenColor
        {
            get { return Color.Red; }
        }

        public override Color secondColor
        {
            get { return Color.FromArgb(255, 255, 51, 51); }
        }

        public override string ToString()
        {
            return "Red";
        }

        public override int mixerHelper
        {
            get { return 10000; }
        }
    }

    public class YellowColour : BlobColour
    {
        public override Color screenColor
        {
            get { return Color.Yellow; }
        }

        public override Color secondColor
        {
            get { return Color.FromArgb(255, 255, 255, 51); }
        }

        public override string ToString()
        {
            return "Yellow";
        }

        public override int mixerHelper
        {
            get { return 100000; }
        }
    }

    public class PurpleColour : BlobColour
    {
        public override Color screenColor
        {
            get { return Color.Purple; }
        }

        public override Color secondColor
        {
            get { return Color.FromArgb(255, 142, 51, 141); }
        }

        public override string ToString()
        {
            return "Purple";
        }

        public override int mixerHelper
        {
            get { return 1000000; }
        }
    }

    public class OrangeColour : BlobColour
    {
        public override Color screenColor
        {
            get { return Color.Orange; }
        }

        public override Color secondColor
        {
            get { return Color.FromArgb(255, 255, 153, 51); }
        }

        public override string ToString()
        {
            return "Orange";
        }

        public override int mixerHelper
        {
            get { return 10000000; }
        }
    }

    public class GreenColour : BlobColour
    {
        public override Color screenColor
        {
            get { return Color.Green; }
        }

        public override Color secondColor
        {
            get { return Color.FromArgb(255, 51, 255, 51);; }
        }

        public override string ToString()
        {
            return "Green";
        }

        public override int mixerHelper
        {
            get { return 100000000; }
        }
    }

    public class ColourMixer
    {
        public static BlobColour mix(BlobColour c1, BlobColour c2)
        {
            int mixed = c1.mixerHelper + c2.mixerHelper;

            switch (mixed)
            {
                    //Whites
                case 2:         //White  + White  = White
                    return new WhiteColour();
                case 11:        //White  + Brown  = Brown
                    return new BrownColour();
                case 101:       //White  + Black  = Black
                    return new BlackColour();
                case 1001:      //White  + Blue   = Blue
                    return new BlueColour();
                case 10001:     //White  + Red    = Red
                    return new RedColour();
                case 100001:    //White  + Yellow = Yellow
                    return new YellowColour();
                case 1000001:   //White  + Purple = Purple
                    return new PurpleColour();
                case 10000001:  //White  + Orange = Orange
                    return new OrangeColour();
                case 100000001: //White  + Green  = Green
                    return new GreenColour();

                    //Browns
                case 20:        //Brown  + Brown  = Brown
                    return new BrownColour();
                case 110:       //Brown  + Black  = Black
                    return new BlackColour();
                case 1010:      //Brown  + Blue   = Orange
                    return new OrangeColour();
                case 10010:     //Brown  + Red    = Green
                    return new GreenColour();
                case 100010:    //Brown  + Yellow = Purple
                    return new PurpleColour();
                case 1000010:   //Brown  + Purple = Yellow
                    return new YellowColour();
                case 10000010:  //Brown  + Orange = Blue
                    return new BlueColour();
                case 100000010: //Brown  + Green  = Red
                    return new RedColour();

                    //Blacks
                case 200:       //Black  + Black  = Black
                case 1100:      //Black  + Blue   = Black
                case 10100:     //Black  + Red    = Black
                case 100100:    //Black  + Yellow = Black
                case 1000100:   //Black  + Purple = Black
                case 10000100:  //Black  + Orange = Black
                case 100000100: //Black  + Green  = Black
                    return new BlackColour();

                    //Blues
                case 2000:      //Blue   + Blue   = Blue
                    return new BlueColour();
                case 11000:     //Blue   + Red    = Purple
                    return new PurpleColour();
                case 101000:    //Blue   + Yellow = Green
                    return new GreenColour();
                case 1001000:   //Blue   + Purple = Brown??
                    return new BrownColour();
                case 10001000:  //Blue   + Orange = Black
                    return new BlackColour();
                case 100001000: //Blue   + Green  = Brown??
                    return new BrownColour();

                    //Reds
                case 20000:     //Red    + Red    = Red
                    return new RedColour();
                case 110000:    //Red    + Yellow = Orange
                    return new OrangeColour();
                case 1010000:   //Red    + Purple = Brown??
                case 10010000:  //Red    + Orange = Brown??
                    return new BrownColour();
                case 100010000: //Red    + Green  = Black
                    return new BlackColour();

                    //Yellows
                case 200000:    //Yellow + Yellow = Yellow
                    return new YellowColour();
                case 1100000:   //Yellow + Purple = Black
                    return new BlackColour();
                case 10100000:  //Yellow + Orange = Brown??
                case 100100000: //Yellow + Green  = Brown??
                    return new BrownColour();

                    //Purples
                case 2000000:   //Purple + Purple = Purple
                    return new PurpleColour();
                case 11000000:  //Purple + Orange = Red
                    return new RedColour();
                case 101000000: //Purple + Green  = Blue
                    return new BlueColour();

                    //Oranges
                case 20000000:  //Orange + Orange = Orange
                    return new OrangeColour();
                case 110000000: //Orange + Green  = Yellow
                    return new YellowColour();
                    
                    //Greens
                case 200000000: //Green  + Green  = Green
                    return new GreenColour();
            }
            return new BlobColour();
        }

        public static BlobColour complement(BlobColour c)
        {
            switch(c.mixerHelper)
            {
                case 1:
                    return new BlackColour();
                case 10:
                    return new BrownColour();
                case 100:
                    return new WhiteColour();
                case 1000:
                    return new OrangeColour();
                case 10000:
                    return new GreenColour();
                case 100000:
                    return new PurpleColour();
                case 1000000:
                    return new YellowColour();
                case 10000000:
                    return new BlueColour();
                case 100000000:
                    return new RedColour();
            }
            return new BlobColour();
        }
    }
}
