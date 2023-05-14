using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPHN.ModelLayer.ObjectQueues
{
    public class ResetPasswordParameter : SendMailParameter
    {
        public Guid AccountId { get; set; }
    }
}
