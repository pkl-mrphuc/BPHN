using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPHN.ModelLayer.Others
{
    public class WhereCondition
    {
        public string Column { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }
    }
}
