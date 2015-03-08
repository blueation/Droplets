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

        private List<Source>[] sourceHistory;

        public History(int entries, List<Source> start)
        {
            this.entries = entries;
            sourceHistory = new List<Source>[entries];
            List<Source> result = new List<Source>();
            foreach (Source s in start)
                result.Add(s.Copy());
            sourceHistory[head] = result;
        }

        public void Clear()
        {
            sourceHistory = new List<Source>[entries];
            head = 0;
            tail = 0;
        }

        public void Add(List<Source> entry)
        {
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
