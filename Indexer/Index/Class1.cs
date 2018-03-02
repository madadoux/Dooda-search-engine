using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Index
{
    class TermClass
    {
        public string term = null;
        public List<int> doc;
        public Dictionary<int, List<int>> postion;
        public int frequncy = 0;
    }
}
