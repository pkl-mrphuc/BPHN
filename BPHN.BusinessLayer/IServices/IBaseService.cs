namespace BPHN.BusinessLayer.IServices
{
    public interface IBaseService
    {
        bool ValidateModelByAttribute(object model, List<string> ignoreProperties);
    }
}
