using BPHN.BusinessLayer.IServices;
using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;
using Microsoft.Extensions.Options;

namespace BPHN.BusinessLayer.ImpServices
{
    public class TimeFrameInfoService : BaseService, ITimeFrameInfoService
    {
        private readonly ITimeFrameInfoRepository _timeFrameInfoRepository;
        public TimeFrameInfoService(
            IServiceProvider provider, 
            IOptions<AppSettings> appSettings,
            ITimeFrameInfoRepository timeFrameInfoRepository) : base(provider, appSettings)
        {
            _timeFrameInfoRepository = timeFrameInfoRepository;
        }

        public async Task<TimeFrameInfo?> GetById(Guid id)
        {
            return await _timeFrameInfoRepository.GetById(id);
        }

        public async Task<List<TimeFrameInfo>> GetByListPitchId(List<Guid> lstPitchId)
        {
            return await _timeFrameInfoRepository.GetByListPitchId(lstPitchId);
        }

        public async Task<List<TimeFrameInfo>> GetByPitchId(Guid pitchId)
        {
            return await _timeFrameInfoRepository.GetByPitchId(pitchId);
        }
    }
}
