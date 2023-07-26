using BPHN.ModelLayer;

namespace BPHN.DataLayer.IRepositories
{
    public interface ITimeFrameInfoRepository
    {
        Task<List<TimeFrameInfo>> GetByPitchId(Guid pitchId);
        Task<List<TimeFrameInfo>> GetByListPitchId(List<Guid> lstPitchId);
        Task<TimeFrameInfo?> GetById(Guid id);
    }
}
