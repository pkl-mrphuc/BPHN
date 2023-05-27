using BPHN.ModelLayer;
using BPHN.ModelLayer.Others;
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
        List<Account> GetPaging(int pageIndex, int pageSize, List<WhereCondition> where);
        object GetCountPaging(int pageIndex, int pageSize, List<WhereCondition> where);
        bool SavePassword(Guid id, string password);
    }
}
