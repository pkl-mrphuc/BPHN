using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPHN.ModelLayer.Others
{
    public class HistoryLogDescription
    {
        public Guid Id { get; set; }
        public Guid ModelId { get; set; }
        public string OldData { get; set; }
        public string NewData { get; set; }
    }
}
