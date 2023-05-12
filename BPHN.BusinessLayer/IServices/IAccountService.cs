using BPHN.ModelLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPHN.BusinessLayer.IServices
{
    public interface IAccountService
    {
        ServiceResultModel Login(Account account);
        ServiceResultModel GetById(Guid id);
        ServiceResultModel RegisterForTenant(Account account);
        ServiceResultModel ResetPassword(string userName);
        ServiceResultModel GetPaging(int pageIndex, int pageSize, string txtSearch);
        ServiceResultModel GetCountPaging(int pageIndex, int pageSize, string txtSearch);
    }
}
