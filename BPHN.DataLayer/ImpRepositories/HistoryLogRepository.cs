using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPHN.DataLayer.ImpRepositories
{
    public class HistoryLogRepository : IHistoryLogRepository
    {
        public object GetCountPaging(int pageIndex, int pageSize, string txtSearch)
        {
            return new { TotalPage = 1, TotalRecordCurrentPage = 10, TotalAllRecords = 100 };
        }

        public List<HistoryLog> GetPaging(int pageIndex, int pageSize, string txtSearch)
        {
            return new List<HistoryLog>();
        }

        public bool Write(HistoryLog history)
        {
            return true;
        }
    }
}
