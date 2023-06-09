using BPHN.ModelLayer;
using BPHN.ModelLayer.Others;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPHN.DataLayer.IRepositories
{
    public interface IHistoryLogRepository
    {
        Task<bool> Write(HistoryLog history);
        Task<List<HistoryLog>> GetPaging(int pageIndex, int pageSize, List<WhereCondition> wheres);
        Task<object> GetCountPaging(int pageIndex, int pageSize, List<WhereCondition> wheres);
    }
}
