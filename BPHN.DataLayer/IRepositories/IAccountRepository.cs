using BPHN.ModelLayer;
using BPHN.ModelLayer.Others;

namespace BPHN.DataLayer.IRepositories
{
    public interface IAccountRepository
    {
        Task<Account?> GetAccountByUserName(string userName);
        Task<bool> CheckExistUserName(string userName);
        (string token, string refreshToken) GetToken(Guid id);
        Guid ValidateToken(string token, bool isRefreshToken);
        Task<Account?> GetAccountById(Guid id);
        Task<bool> RegisterForTenant(Account account);
        Task<bool> UpdateTenant(Account account);
        Task<IEnumerable<Account>> GetPaging(Guid accountId, RoleEnum role, int pageIndex, int pageSize, string txtSearch);
        Task<object> GetCountPaging(Guid accountId, RoleEnum role, int pageIndex, int pageSize, string txtSearch);
        Task<int> GetTotalRecord(Guid accountId, RoleEnum role, string txtSearch);
        Task<bool> SavePassword(Guid id, string password);
        Task<IEnumerable<Guid>> GetRelationIds(Guid id);
        Task<List<Account>> GetAll();
        Task SaveToken(Guid id, string token, string refreshToken);
    }
}
