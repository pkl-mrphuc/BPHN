﻿using BPHN.ModelLayer;
using BPHN.ModelLayer.Others;

namespace BPHN.DataLayer.IRepositories
{
    public interface IAccountRepository
    {
        Task<Account?> GetAccountByUserName(string userName);
        Task<bool> CheckExistUserName(string userName);
        string GetToken(string id);
        Task<Account?> GetAccountById(Guid id);
        Task<bool> RegisterForTenant(Account account);
        Task<List<Account>> GetPaging(int pageIndex, int pageSize, string txtSearch, List<WhereCondition> where);
        Task<object> GetCountPaging(int pageIndex, int pageSize, string txtSearch, List<WhereCondition> where);
        Task<bool> SavePassword(Guid id, string password);
    }
}
