using BPHN.ModelLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPHN.BusinessLayer.IServices
{
    public interface IHistoryLogService
    {
        Task<ServiceResultModel> GetPaging(int pageIndex, int pageSize, string txtSearch);
        Task<ServiceResultModel> GetCountPaging(int pageIndex, int pageSize, string txtSearch);
        Task<ServiceResultModel> Write(HistoryLog history, Account? context);
    }
}
