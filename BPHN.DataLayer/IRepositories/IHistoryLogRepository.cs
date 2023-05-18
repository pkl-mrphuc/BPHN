using BPHN.ModelLayer;
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
        List<HistoryLog> GetPaging(int pageIndex, int pageSize, string txtSearch);
        object GetCountPaging(int pageIndex, int pageSize, string txtSearch);
    }
}
