using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Droplets
{
    /// <summary>
    /// BlobSize is a class used to abstract to size properties
    /// </summary>
    public class BlobSize
    {
        /// <summary>
        /// allows an indirect comparison between sizes
        /// </summary>
        virtual public int toInt
        {
            get { return 0; } //should never get this.
        }

        /// <summary>
        /// returns the maximum allowed distance between anchors for certain sizes
        /// </summary>
        /// <returns></returns>
        virtual public float getMaxStretch
        {
            get { return 0; } //should never get this.
        }

        virtual public float getRadius
        {
            get { return 0; } //should never get this.
        }

        virtual public float getRetractionPerUpdate
        {
            get { return 0; } //should never get this.
        }

        virtual public float getArbitraryEccentricityValue
        {
            get { return 0; } //should never get this.
        }

        /// <summary>
        /// allows the conversion back to the abstract sizing
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public BlobSize fromInt(int size)
        {
            if (size == 1)
                return new SmallSize();
            if (size == 2)
                return new MediumSize();
            if (size == 3)
                return new LargeSize();
            return new BlobSize();
        }
    }

    public class SmallSize : BlobSize
    {
        public override int toInt
        {
            get { return 1; }
        }

        public override float getMaxStretch
        {
            get { return 100; }
        }

        public override float getRadius
        {
            get { return 25; }
        }

        public override float getRetractionPerUpdate
        {
            get { return 1.6f; }
        }

        public override float getArbitraryEccentricityValue
        {
            get { return 1.5f; }
        }
    }

    public class MediumSize : BlobSize
    {
        public override int toInt
        {
            get { return 2; }
        }

        public override float getMaxStretch
        {
            get { return 140; }
        }

        public override float getRadius
        {
            get { return 35; }
        }

        public override float getRetractionPerUpdate
        {
            get { return 1.75f; }
        }

        public override float getArbitraryEccentricityValue
        {
            get { return 2.1f; }
        }
    }

    public class LargeSize : BlobSize
    {
        public override int toInt
        {
            get { return 3; }
        }

        public override float getMaxStretch
        {
            get { return 180; }
        }

        public override float getRadius
        {
            get { return 45; }
        }

        public override float getRetractionPerUpdate
        {
            get { return 1.75f; }
        }

        public override float getArbitraryEccentricityValue
        {
            get { return 2.8f; }
        }
    }
}
