using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPHN.ModelLayer
{
    public class BookingDetail : BaseModel
    {
        public DateTime MatchDate { get; set; }
        public double Deposit { get; set; }
        public Guid BookingId { get; set; }
    }
}
