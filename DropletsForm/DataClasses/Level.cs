using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace Droplets
{
    /// <summary>
    /// An object which holds all the necessary data of a single level
    /// </summary>
    class Level
    {
        public int nr;
        public string refname;
        public string name;
        public bool onlyforced;
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
