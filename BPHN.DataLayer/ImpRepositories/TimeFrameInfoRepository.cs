using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;
using Dapper;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPHN.DataLayer.ImpRepositories
{
    public class TimeFrameInfoRepository : BaseRepository, ITimeFrameInfoRepository
    {
        public TimeFrameInfoRepository(IOptions<AppSettings> appSettings) : base(appSettings)
        {
        }

        public List<TimeFrameInfo> GetByPitchId(Guid pitchId)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var dic = new Dictionary<string, object>();
                dic.Add("@pitchId", pitchId);
                return connection.Query<TimeFrameInfo>("select * from time_frame_infos where PitchId = @pitchId", dic).ToList();
            }
        }
    }
}
