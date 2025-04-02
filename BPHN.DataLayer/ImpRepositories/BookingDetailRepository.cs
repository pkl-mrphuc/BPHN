using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;
using BPHN.ModelLayer.Others;
using Dapper;
using Microsoft.Extensions.Options;

namespace BPHN.DataLayer.ImpRepositories
{
    public class BookingDetailRepository : BaseRepository, IBookingDetailRepository
    {
        public BookingDetailRepository(IOptions<AppSettings> appSettings) : base(appSettings)
        {
        }

        public async Task<bool> Cancel(string id)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var affect = await connection.ExecuteAsync(Query.BOOKING_DETAIL__UPDATE_STATUS, new Dictionary<string, object>
                {
                    { "@status", BookingStatusEnum.CANCEL.ToString() },
                    { "@id", id }
                });
                return affect > 0 ? true : false;
            }
        }

        public async Task<List<BookingDetail>> GetByBookingId(Guid bookingId)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var data = await connection.QueryAsync<BookingDetail>(Query.BOOKING_DETAIL__GET_BY_BOOKING_ID, new Dictionary<string, object>()
                {
                    { "@bookingId", bookingId }
                });
                return data.ToList();
            }
        }

        public async Task<IEnumerable<CalendarEvent>> GetByDate(string date, Guid[] relationIds)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var lstBookingDetail = await connection.QueryAsync<CalendarEvent>(Query.BOOKING_DETAIL__GET_BY_DATE, new Dictionary<string, object>
                {
                    { "@status0", BookingStatusEnum.SUCCESS.ToString() },
                    { "@status1", BookingStatusEnum.PENDING.ToString() },
                    { "@accountId", relationIds },
                    { "@startDate", $"{date} 00:00:00" },
                    { "@endDate", $"{date} 23:59:59" },
                });
                return lstBookingDetail ?? Enumerable.Empty<CalendarEvent>();
            }
        }

        public async Task<BookingDetail?> GetById(string id)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var data = await connection.QueryAsync<BookingDetail>(Query.BOOKING_DETAIL__GET_BY_ID, new Dictionary<string, object>
                {
                    { "@id", id }
                });
                return data.FirstOrDefault();
            }
        }

        public async Task<IEnumerable<CalendarEvent>> GetEventsByRangeDate(DateTime startDate, DateTime endDate, Guid pitchId, string nameDetail)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var lstBookingDetail = await connection.QueryAsync<CalendarEvent>(Query.BOOKING_DETAIL__GET_CALENDAR_EVENTS, new Dictionary<string, object>
                {
                    { "@startDate", startDate.ToString("yyyy-MM-dd 00:00:00") },
                    { "@endDate", endDate.ToString("yyyy-MM-dd 23:59:59") },
                    { "@pitchId", pitchId },
                    { "@nameDetail", nameDetail },
                });
                return lstBookingDetail ?? Enumerable.Empty<CalendarEvent>();
            }
        }

        public async Task<List<CalendarEvent>> GetByRangeDate(string startDate, string endDate, string pitchId, string nameDetail)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var dic = new Dictionary<string, object>();
                dic.Add("@status0", BookingStatusEnum.SUCCESS.ToString());
                dic.Add("@startDate", $"{startDate} 00:00:00");
                dic.Add("@endDate", $"{endDate} 23:59:59");
                dic.Add("@pitchId", pitchId);

                var query = @"select bd.*, b.PitchId, tfi.TimeBegin as Start, tfi.TimeEnd as End, b.NameDetail as Stadium, b.PhoneNumber as PhoneNumber  from booking_details bd 
                                                inner join bookings b on b.Id = bd.BookingId
                                                inner join time_frame_infos tfi on b.TimeFrameInfoId = tfi.Id
                                                inner join pitchs p on p.Id = b.PitchId and p.Id = @pitchId
                                                where   bd.Status in (@status0) and
                                                        bd.MatchDate between @startDate and @endDate";

                if (!string.IsNullOrWhiteSpace(nameDetail))
                {
                    dic.Add("@nameDetail", nameDetail);

                    query = @"select bd.*, b.PitchId, tfi.TimeBegin as Start, tfi.TimeEnd as End, b.NameDetail as Stadium, b.PhoneNumber as PhoneNumber  from booking_details bd 
                                                inner join bookings b on b.Id = bd.BookingId
                                                inner join time_frame_infos tfi on b.TimeFrameInfoId = tfi.Id
                                                inner join pitchs p on p.Id = b.PitchId and p.Id = @pitchId
                                                where   bd.Status in (@status0) and b.NameDetail = @nameDetail and
                                                        bd.MatchDate between @startDate and @endDate";
                }

                var lstBookingDetail = (await connection.QueryAsync<CalendarEvent>(query, dic)).ToList();
                return lstBookingDetail;
            }
        }

        public async Task<List<BookingDetail>> GetInRangeDate(Guid accountId, DateTime startDate, DateTime endDate)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var dic = new Dictionary<string, object>();
                dic.Add("@accountId", accountId);
                dic.Add("@startDate", startDate.ToString("yyyy-MM-dd"));
                dic.Add("@endDate", endDate.ToString("yyyy-MM-dd"));
                dic.Add("@status0", BookingStatusEnum.SUCCESS.ToString());
                var query = @"select bd.* from booking_details bd inner join bookings b on b.Id = bd.BookingId where bd.Status in (@status0) and b.AccountId = @accountId and bd.MatchDate between @startDate and @endDate";
                var data = await connection.QueryAsync<BookingDetail>(query, dic);
                return data.ToList();
            }
        }

        public async Task<bool> UpdateMatch(CalendarEvent eventInfo)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var affect = await connection.ExecuteAsync(Query.BOOKING_DETAIL__UPDATE_MATCH, new Dictionary<string, object>
                {
                    { "@id", eventInfo.Id},
                    { "@teamA", eventInfo.TeamA},
                    { "@teamB", eventInfo.TeamB},
                    { "@note", eventInfo.Note},
                    { "@deposit", eventInfo.Deposit},
                });
                return affect > 0 ? true : false;
            }
        }
    }
}
