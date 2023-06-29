using BPHN.ModelLayer;

namespace BPHN.BusinessLayer.IServices
{
    public interface IContextService
    {
        Account? GetContext();
        string GetIPAddress();
    }
}
