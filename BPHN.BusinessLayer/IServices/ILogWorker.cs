namespace BPHN.BusinessLayer.IServices
{
    public interface ILogWorker
    {
        Task Handle(string dataJson);
    }
}
