using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPHN.ModelLayer
{
    public class HistoryLog : BaseModel
    {
        public ActionEnum ActionType { get; set; }
        public string ActionName { get; set; }
        public string Description { get; set; }
        public string Actor { get; set; }
        public Guid ActorId { get; set; }
    }
}
