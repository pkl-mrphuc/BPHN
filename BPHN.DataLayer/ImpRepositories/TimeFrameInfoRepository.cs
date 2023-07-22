using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;
using Dapper;
using Microsoft.Extensions.Options;

namespace BPHN.DataLayer.ImpRepositories
{
    public class TimeFrameInfoRepository : BaseRepository, ITimeFrameInfoRepository
    {
        public TimeFrameInfoRepository(IOptions<AppSettings> appSettings) : base(appSettings)
        {
        }

        public async Task<List<TimeFrameInfo>> GetByPitchId(Guid pitchId)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var dic = new Dictionary<string, object>();
                dic.Add("@pitchId", pitchId);
                var lstTimeFrameInfo = await connection.QueryAsync<TimeFrameInfo>("select Id, SortOrder, Name, TimeBegin, TimeEnd, Price from time_frame_infos where PitchId = @pitchId order by SortOrder", dic);
                return lstTimeFrameInfo.ToList();
            }
        }

        public async Task<List<TimeFrameInfo>> GetByListPitchId(List<Guid> lstPitchId)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var dic = new Dictionary<string, object>();
                dic.Add("@pitchId", lstPitchId.ToArray());
                var lstTimeFrameInfo = await connection.QueryAsync<TimeFrameInfo>("select Id, SortOrder, Name, TimeBegin, TimeEnd, Price, PitchId from time_frame_infos where PitchId in @pitchId order by SortOrder", dic);
                return lstTimeFrameInfo.ToList();
            }
        }
    }
}
