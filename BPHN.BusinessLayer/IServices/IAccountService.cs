using BPHN.ModelLayer;

namespace BPHN.BusinessLayer.IServices
{
    public interface IAccountService
    {
        Task<ServiceResultModel> Login(Account account);
        Task<ServiceResultModel> GetById(Guid id);
        Task<ServiceResultModel> RegisterForTenant(Account account);
        Task<ServiceResultModel> ResetPassword(string userName);
        Task<ServiceResultModel> GetPaging(int pageIndex, int pageSize, string txtSearch);
        Task<ServiceResultModel> GetCountPaging(int pageIndex, int pageSize, string txtSearch);
        Task<ServiceResultModel> SubmitSetPassword(string code, string password, string userName);
        ServiceResultModel ValidateToken(string token);
        ServiceResultModel GetTokenInfo(string token);
        Task<ServiceResultModel> ChangePassword(Account account);
        Task<ServiceResultModel> GetInstance(string id);
        Task<ServiceResultModel> Refresh();
    }
}
