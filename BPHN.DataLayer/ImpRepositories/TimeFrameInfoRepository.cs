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
                var lstTimeFrameInfo = await connection.QueryAsync<TimeFrameInfo>(Query.TIME_FRAME__GET_BY_PITCH_ID, new Dictionary<string, object>
                {
                    { "@pitchId", pitchId }
                });
                return lstTimeFrameInfo.ToList();
            }
        }

        public async Task<List<TimeFrameInfo>> GetByListPitchId(List<Guid> lstPitchId)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var lstTimeFrameInfo = await connection.QueryAsync<TimeFrameInfo>(Query.TIME_FRAME__GET_BY_LIST_PITCH_ID, new Dictionary<string, object>
                {
                    { "@pitchId", lstPitchId.ToArray() }
                });
                return lstTimeFrameInfo.ToList();
            }
        }

        public async Task<TimeFrameInfo?> GetById(Guid id)
        {
            if (id.Equals(Guid.Empty)) return null;
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var timeFrameInfo = await connection.QueryFirstOrDefaultAsync<TimeFrameInfo>(Query.TIME_FRAME__GET_BY_ID, new Dictionary<string, object>
                {
                    { "@id", id }
                });
                return timeFrameInfo;
            }
        }
    }
}
