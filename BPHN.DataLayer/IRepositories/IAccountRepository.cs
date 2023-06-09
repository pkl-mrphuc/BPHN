using BPHN.ModelLayer;

namespace BPHN.DataLayer.IRepositories
{
    public interface IAccountRepository
    {
        Task<Account?> GetAccountByUserName(string userName);
        Task<bool> CheckExistUserName(string userName);
        string GetToken(string id);
        Task<Account?> GetAccountById(Guid id);
        Task<bool> RegisterForTenant(Account account);
        Task<List<Account>> GetPaging(int pageIndex, int pageSize, string txtSearch);
        Task<object> GetCountPaging(int pageIndex, int pageSize, string txtSearch);
        Task<bool> SavePassword(Guid id, string password);
    }
}
