using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Droplets
{
    /// <summary>
    /// BlobSize is a class used to abstract to size properties
    /// </summary>
    public abstract class BlobSize
    {
        /// <summary>
        /// allows an indirect comparison between sizes
        /// </summary>
        virtual public int toInt
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
            return null;
        }
    }

    public class SmallSize : BlobSize
    {
        public override int toInt
        {
            get { return 1; }
        }
    }

    public class MediumSize : BlobSize
    {
        public override int toInt
        {
            get { return 2; }
        }
    }

    public class LargeSize : BlobSize
    {
        public override int toInt
        {
            get { return 3; }
        }
    }
}
