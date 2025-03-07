using BPHN.ModelLayer;

namespace BPHN.BusinessLayer.IServices
{
    public interface ITimeFrameInfoService
    {
        Task<TimeFrameInfo?> GetById(Guid id);
        Task<List<TimeFrameInfo>> GetByPitchId(Guid pitchId);
        Task<List<TimeFrameInfo>> GetByListPitchId(List<Guid> lstPitchId);
    }
}
