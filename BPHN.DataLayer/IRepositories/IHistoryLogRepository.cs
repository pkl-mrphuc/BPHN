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
        bool Write(HistoryLog history);
        List<HistoryLog> GetPaging(int pageIndex, int pageSize, List<WhereCondition> wheres);
        object GetCountPaging(int pageIndex, int pageSize, List<WhereCondition> wheres);
    }
}
