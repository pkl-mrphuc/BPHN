using BPHN.ModelLayer;
using BPHN.ModelLayer.Others;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPHN.DataLayer.IRepositories
{
    public interface IPitchRepository
    {
        Task<List<Pitch>> GetPaging(int pageIndex, int pageSize, List<WhereCondition> where);
        Task<object> GetCountPaging(int pageIndex, int pageSize, List<WhereCondition> where);
        Task<Pitch?> GetById(string id);
        Task<bool> Insert(Pitch pitch);
        Task<bool> Update(Pitch pitch);
    }
}
