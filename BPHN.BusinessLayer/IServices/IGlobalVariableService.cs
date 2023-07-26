using BPHN.ModelLayer;

namespace BPHN.BusinessLayer.IServices
{
    public interface IGlobalVariableService
    {
        void Start();
        void Reset();
        Dictionary<Guid, Account> AccountSystem { get; }
    }
}
