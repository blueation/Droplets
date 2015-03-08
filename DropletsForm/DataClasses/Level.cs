using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace Droplets
{
    class Level
    {
        public int nr;
        public string refname;
        public string name;
        public List<SubmitZone> submitzones;
        public List<Source> sources;

        public Level(string refname)
        {
            this.refname = refname;
            submitzones = new List<SubmitZone>();
            sources = new List<Source>();
        }
    }
}
