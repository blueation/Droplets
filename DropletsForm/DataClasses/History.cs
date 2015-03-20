using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Droplets
{
    class History
    {
        private int head = 0;
        private int tail = 0;
        private int entries = 0;
        private List<Source> start;

        private List<Source>[] sourceHistory;

        public History(int entries, List<Source> start)
        {
            this.start = start;
            this.entries = entries;
            sourceHistory = new List<Source>[entries];
            List<Source> result = new List<Source>();
            foreach (Source s in start)
                result.Add(s.Copy());
            sourceHistory[head] = result;
        }

        public void Clear()
        {
            head = 0;
            tail = 0;
            sourceHistory = new List<Source>[entries];
            List<Source> result = new List<Source>();
            foreach (Source s in start)
                result.Add(s.Copy());
            sourceHistory[head] = result;
        }

        public void Add(List<Source> entry)
        {
            bool different = entry.Count != sourceHistory[head].Count;
            for (int i = 0; !different && i < entry.Count; i++)
            {
                if (entry[i].Active != sourceHistory[head][i].Active
                    || entry[i].SourceAnchor.X != sourceHistory[head][i].SourceAnchor.X
                    || entry[i].SourceAnchor.Y != sourceHistory[head][i].SourceAnchor.Y
                    || entry[i].SourceColour.ToString() != sourceHistory[head][i].SourceColour.ToString()
                    || entry[i].SourceSize.toInt != sourceHistory[head][i].SourceSize.toInt
                    )
                    different = true;
            }

            if (!different)
                return;

            List<Source> result = new List<Source>();
            foreach (Source s in entry)
                result.Add(s.Copy());
            head++;
            head %= entries;
            sourceHistory[head] = result;
            if (head == tail)
            {
                tail++;
                tail %= entries;
            }
        }

        public List<Source> Retrieve()
        {
            int retrieveloc = head;
            if (head != tail)
                head--;
            if (head < 0)
                head += entries;
            return sourceHistory[retrieveloc];
        }
    }
}
