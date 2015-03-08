using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Droplets
{
    class LevelLoader
    {
        public static Level LoadLevel(string filepath)
        {
            Level result = new Level();
            using (TextReader sr = new StreamReader(filepath))
            {
                string line = sr.ReadLine();
                if (line != "DropletsGame") return null;

                line = sr.ReadLine();
                string[] parts = line.Split(' ');
                result.nr = int.Parse(parts[0]);
                result.name = line.Substring(parts[0].Length + 1);

                line = sr.ReadLine();
                parts = line.Split(' ');
                if (parts[0] != "SubmitZones") return null;
                int i = int.Parse(parts[1]);

                for (int x = 0; x < i; x++)
                {
                    line = sr.ReadLine();
                    parts = line.Split(' ');
                    SubmitZone newzone = new SubmitZone();
                    newzone.colour = new BlobColour().fromString(parts[0]);
                    int j = int.Parse(parts[1]);

                    for (int y = 0; y < j; y++)
                    {
                        line = sr.ReadLine();
                        parts = line.Split(' ');
                        SubSubmitZone newsubzone;
                        if (parts[0] == "Rectangle")
                        {
                            newsubzone = new SquareSubmitZone(int.Parse(parts[1]), int.Parse(parts[2]), int.Parse(parts[3]), int.Parse(parts[4]), newzone);
                            newzone.AddZone(newsubzone);
                        }
                        else //if (parts[0] == "Circle")
                        {
                            newsubzone = new CircleSubmitZone(int.Parse(parts[1]), int.Parse(parts[2]), int.Parse(parts[3]), newzone);
                            newzone.AddZone(newsubzone);
                        }
                    }
                    result.submitzones.Add(newzone);
                }

                line = sr.ReadLine();
                parts = line.Split(' ');
                if (parts[0] != "Sources") return null;
                i = int.Parse(parts[1]);

                for(int x = 0; x < i; x++)
                {
                    line = sr.ReadLine();
                    parts = line.Split(' ');
                    Source newsource = new Source(new BlobColour().fromString(parts[0]), new BlobSize().fromInt(int.Parse(parts[1])), new Vector2(int.Parse(parts[2]), int.Parse(parts[3])));
                    result.sources.Add(newsource);
                }
            }
            return result;
        }

        public static string[] AllPathsOfDirectory(string folderpath)
        {
            return Directory.GetFiles(folderpath);
        }
    }
}
