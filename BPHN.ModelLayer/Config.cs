using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPHN.ModelLayer
{
    public class Config : BaseModel
    {
        public string Key { get; set; }
        public object? Value { get; set; }
        public string AccountId { get; set; }
    }
}
