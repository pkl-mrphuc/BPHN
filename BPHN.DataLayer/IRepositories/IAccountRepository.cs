using BPHN.ModelLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPHN.DataLayer.IRepositories
{
    public interface IAccountRepository
    {
        Account? GetAccountByUserName(string userName);
        bool CheckExistUserName(string userName);
        string GetToken(string id);
        Account? GetAccountById(Guid id);
        bool RegisterForTenant(Account account);
        List<Account> GetPaging(int pageIndex, int pageSize, string txtSearch);
        object GetCountPaging(int pageIndex, int pageSize, string txtSearch);
    }
}
