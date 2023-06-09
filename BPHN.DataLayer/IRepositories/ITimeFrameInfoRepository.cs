using BPHN.ModelLayer;

namespace BPHN.DataLayer.IRepositories
{
    public interface ITimeFrameInfoRepository
    {
        Task<List<TimeFrameInfo>> GetByPitchId(Guid pitchId);
    }
}
