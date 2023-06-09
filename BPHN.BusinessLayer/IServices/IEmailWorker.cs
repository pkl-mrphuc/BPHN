namespace BPHN.BusinessLayer.IServices
{
    public interface IEmailWorker
    {
        Task Handle(string parameter);
    }
}
