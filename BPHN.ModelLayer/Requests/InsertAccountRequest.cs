using System.ComponentModel.DataAnnotations;

namespace BPHN.ModelLayer.Requests
{
    public class InsertAccountRequest
    {
        public string UserName { get; set; }
        public string Gender { get; set; }
        public string PhoneNumber { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
    }
}
