namespace BPHN.BusinessLayer.IServices
{
    public interface ILogService
    {
        void LogDebug(string message);
        void LogError(string message);
        void LogInformation(string message);
        void LogWarning(string message);
    }
}
