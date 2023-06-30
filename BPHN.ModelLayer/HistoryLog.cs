using BPHN.ModelLayer.Others;
using System.ComponentModel.DataAnnotations;

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
        public string Entity { get; set; }
        public string IPAddress { get; set; }
        public HistoryLogDescription? Data { get; set; }
    }
}
