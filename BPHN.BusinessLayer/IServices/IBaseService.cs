namespace BPHN.BusinessLayer.IServices
{
    public interface IBaseService
    {
        bool ValidateModelByAttribute(object model, List<string> ignoreProperties);
        string BuildDescriptionForHistoryLog<T>(List<T>? oldData, List<T> newData);
        string BuildDescriptionForHistoryLog<T>(T? oldData, T newData);
    }
}
