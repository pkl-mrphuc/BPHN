﻿using BPHN.DataLayer.IRepositories;
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
                var dic = new Dictionary<string, object>();
                dic.Add("@status", BookingStatusEnum.CANCEL.ToString());
                dic.Add("@id", id);
                var query = "update booking_details set Status = @status where Id = @id";
                var affect = await connection.ExecuteAsync(query, dic);
                return affect > 0 ? true : false;
            }
        }

        public async Task<List<CalendarEvent>> GetByDate(string date, Guid accountId)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var dic = new Dictionary<string, object>();
                dic.Add("@status0", BookingStatusEnum.SUCCESS.ToString());
                dic.Add("@accountId", accountId);
                dic.Add("@startDate", $"{date} 00:00:00");
                dic.Add("@endDate", $"{date} 23:59:59");
                var query = @"select bd.*, b.PitchId, tfi.TimeBegin as Start, tfi.TimeEnd as End, b.NameDetail as Stadium, b.PhoneNumber as TeamA  from booking_details bd 
                                                inner join bookings b on b.Id = bd.BookingId
                                                inner join time_frame_infos tfi on b.TimeFrameInfoId = tfi.Id
                                                where   bd.Status in (@status0) and 
                                                        b.AccountId = @accountId and 
                                                        bd.MatchDate between @startDate and @endDate";
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
    }
}
