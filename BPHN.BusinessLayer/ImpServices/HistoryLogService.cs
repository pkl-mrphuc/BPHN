using BPHN.BusinessLayer.IServices;
using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPHN.BusinessLayer.ImpServices
{
    public class HistoryLogService : IHistoryLogService
    {
        private readonly IHistoryLogRepository _historyLogRepository;
        public HistoryLogService(IHistoryLogRepository historyLogRepository)
        {
            _historyLogRepository = historyLogRepository;
        }
        public ServiceResultModel GetCountPaging(int pageIndex, int pageSize, string txtSearch)
        {
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize <= 0 || pageSize > 100) pageSize = 50;

            var resultCountPaging = _historyLogRepository.GetCountPaging(pageIndex, pageSize, txtSearch);
            return new ServiceResultModel()
            {
                 Success = true,
                 Data = resultCountPaging
            };
        }

        public ServiceResultModel GetPaging(int pageIndex, int pageSize, string txtSearch)
        {
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize <= 0 || pageSize > 100) pageSize = 50;

            var resultPaging = _historyLogRepository.GetPaging(pageIndex, pageSize, txtSearch);
            return new ServiceResultModel()
            {
                Success = true,
                Data = resultPaging
            };
        }

        public ServiceResultModel Write(HistoryLog history)
        {
            return new ServiceResultModel()
            {
                Success = true,
                Data = _historyLogRepository.Write(history)
            };
        }
    }
}
