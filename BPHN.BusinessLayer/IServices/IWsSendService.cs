namespace BPHN.BusinessLayer.IServices
{
    public interface IWsSendService
    {
        Task SERVER_AfterClientLoginSuccess(string accountId, string connectionId);
        Task SERVER_ConfirmOtherClientLogin();
    }
}
