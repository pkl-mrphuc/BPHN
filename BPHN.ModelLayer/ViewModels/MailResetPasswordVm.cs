using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPHN.ModelLayer.ViewModels
{
    public class MailResetPasswordVm
    {
        public Guid AccountId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Key { get; set; }
    }
}
