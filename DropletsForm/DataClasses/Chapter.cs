﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Droplets
{
    /// <summary>
    /// An object to hold multiple levels
    /// </summary>
    class Chapter
    {
        public List<Level> levels = new List<Level>();
        public string name;
        public int number;

        public Chapter(string name, int number)
        {
            this.name = name;
            this.number = number;
        }
    }
}
