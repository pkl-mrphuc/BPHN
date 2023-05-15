using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPHN.ModelLayer.Others
{
    public class ExpireResetPasswordModel
    {
        public DateTime ExpireTime { get; set; }
        public string AccountId { get; set; }
        public string Password { get; set; }
    }
}
