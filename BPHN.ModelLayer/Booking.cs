using BPHN.ModelLayer.Attributes;
using System.ComponentModel.DataAnnotations;

namespace BPHN.ModelLayer
{
    public class Booking : BaseModel, ICloneable
    {
        public int BookingCode { get; set; }
        [Required]
        [MaxLength(255)]
        public string PhoneNumber { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public bool IsRecurring { get; set; } = false;
        public DateTime BookingDate { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Range(0, 6)]
        public int? Weekendays { get; set; }
        public string Status { get; set; }
        [Required]
        [IgnoreLog]
        public Guid? TimeFrameInfoId { get; set; }
        [IgnoreLog]
        public TimeFrameInfo? TimeFrameInfo { get; set; }
        public string TimeFrameInfoName { get; set; }
        [Required]
        [IgnoreLog]
        public Guid? PitchId { get; set; }
        [IgnoreLog]
        public Pitch? Pitch { get; set; }
        public string PitchName { get; set; }
        [Required]
        [MaxLength(255)]
        public string NameDetail { get; set; }
        [IgnoreLog]
        public string BookingDetailIds { get; set; }
        [IgnoreLog]
        public List<BookingDetail> BookingDetails { get; set; }
        [IgnoreLog]
        public Guid AccountId { get; set; }
        [IgnoreLog]
        public Account Account { get; set; }

        public object Clone()
        {
            return new Booking()
            {
                StartDate = this.StartDate,
                EndDate = this.EndDate,
                AccountId = this.AccountId,
                BookingDetails = this.BookingDetails,
                PitchId = this.PitchId,
                Account = this.Account,
                BookingDate = this.BookingDate,
                BookingDetailIds = this.BookingDetailIds,
                Id = this.Id,
                Email = this.Email,
                NameDetail = this.NameDetail,
                PhoneNumber = this.PhoneNumber,
                IsRecurring = this.IsRecurring,
                Pitch = this.Pitch,
                PitchName = this.PitchName,
                Status = this.Status,
                TimeFrameInfo = this.TimeFrameInfo,
                TimeFrameInfoId = this.TimeFrameInfoId,
                TimeFrameInfoName = this.TimeFrameInfoName,
                Weekendays = this.Weekendays,
                CreatedBy = this.CreatedBy,
                CreatedDate = this.CreatedDate,
                ModifiedBy = this.ModifiedBy,
                ModifiedDate = this.ModifiedDate
            };
        }
    }
}
