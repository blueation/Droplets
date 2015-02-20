using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Droplets
{
    //TODO: This is currently only WIP and only contains any data necessary for reference right now.
    abstract class Source
    {
        public BlobColour SourceColour;
        public BlobSize SourceSize;
        public Tuple<int, int> SourceAnchor;
        public Tuple<int, int> ExtensionAnchor;
    }
}
