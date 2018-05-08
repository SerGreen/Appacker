using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLib
{
    public class ProgressReport
    {
        public ProgressReport(int current, int total)
        {
            Current = current;
            Total = total;
        }

        public int Current { get; set; }
        public int Total { get; set; }
    }
}
