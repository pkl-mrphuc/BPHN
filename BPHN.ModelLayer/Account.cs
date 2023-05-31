using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPHN.ModelLayer
{
    public class Account : BaseModel
    {
        [Required]
        [MaxLength(255)]
        public string UserName { get; set; }
        [Required]
        [MaxLength(500)]
        public string Password { get; set; }
        public GenderEnum Gender { get; set; }
        [Required]
        [MaxLength(255)]
        public string PhoneNumber { get; set; }
        [Required]
        [MaxLength(255)]
        public string FullName { get; set; }
        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; }
        public RoleEnum Role { get; set; }
        public string Token { get; set; }
    }
}
