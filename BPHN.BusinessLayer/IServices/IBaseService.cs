using BPHN.ModelLayer;

namespace BPHN.BusinessLayer.IServices
{
    public interface IBaseService
    {
        bool ValidateModelByAttribute(object model, params string[] ignoreProperties);
        string BuildLinkDescription(Guid historyLogId);
    }
}
