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
        string name;
        private List<SubmitZone> submitzone;

        public void fromFile(string filepath)
        {
            TextReader sr = new StreamReader(filepath);

            string file = sr.ReadToEnd();
            string[] fileLines = file.Split('\n');

            

        }
    }
}
