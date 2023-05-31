using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPHN.ModelLayer
{
    public class HistoryLog : BaseModel
    {
        public ActionEnum ActionType { get; set; }
        [MaxLength(255)]
        public string ActionName { get; set; }
        [MaxLength(int.MaxValue)]
        public string Description { get; set; }
        [MaxLength(255)]
        public string Actor { get; set; }
        public Guid ActorId { get; set; }
    }
}
