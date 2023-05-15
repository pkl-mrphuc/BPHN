using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPHN.ModelLayer.ViewModels
{
    public class MailVm<T>
    {
        public dynamic ViewBag { get; set; }
        public T Model { get; set; }
    }
}
