using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPHN.ModelLayer
{
    public class Config : BaseModel
    {
        [MaxLength(255)]
        public string Key { get; set; }
        [MaxLength(int.MaxValue)]
        public string Value { get; set; }
        public Guid AccountId { get; set; }
    }
}
