using System.ComponentModel.DataAnnotations;

namespace BPHN.ModelLayer.Requests
{
    public sealed class GetCalendarEventRequest
    {
        public string NameDetail { get; set; }
        public Guid PitchId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
