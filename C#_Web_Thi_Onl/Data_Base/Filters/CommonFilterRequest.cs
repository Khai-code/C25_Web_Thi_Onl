using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Base.Filters
{
    public class CommonFilterRequest
    {
        public Dictionary<string, string> Filters { get; set; } = new();
        public object Entity { get; set; }
    }
}
