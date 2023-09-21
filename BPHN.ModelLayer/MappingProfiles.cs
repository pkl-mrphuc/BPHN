using AutoMapper;
using BPHN.ModelLayer;
using BPHN.ModelLayer.Others;
using BPHN.ModelLayer.Requests;
using BPHN.ModelLayer.Responses;

namespace BPHN.WebAPI.Models
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles() 
        {
            CreateMap<LoginRequest, Account>();
            CreateMap<InsertAccountRequest, Account>();
            CreateMap<ChangePasswordRequest, Account>();

            CreateMap<CheckFreeTimeFrameRequest, Booking>();
            CreateMap<FindBlankRequeset, Booking>();
            CreateMap<InsertBookingRequest, Booking>();

            CreateMap<InsertPitchRequest, Pitch>();
            CreateMap<UpdatePitchRequest, Pitch>();

            CreateMap<SaveConfigRequest, Config>();

            CreateMap<SavePermissionRequest, Permission>();

            CreateMap<Account, AccountRespond>();
            CreateMap<CalendarEvent, CalendarEventRespond>();
            CreateMap<Booking, BookingRespond>();
            CreateMap<Config, ConfigRespond>();
            CreateMap<HistoryLog, HistoryLogRespond>();
            CreateMap<Notification, NotificationRespond>();
            CreateMap<Permission, PermissionRespond>();
        }
    }
}
