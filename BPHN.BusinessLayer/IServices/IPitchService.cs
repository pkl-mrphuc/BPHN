﻿using BPHN.ModelLayer;

namespace BPHN.BusinessLayer.IServices
{
    public interface IPitchService
    {
        Task<ServiceResultModel> Insert(Pitch pitch);
        Task<ServiceResultModel> Update(Pitch pitch);
        Task<ServiceResultModel> GetInstance(string id);
        Task<ServiceResultModel> GetPaging(int pageIndex, int pageSize, string txtSearch, string accountId, bool hasDetail = false);
        Task<ServiceResultModel> GetCountPaging(int pageIndex, int pageSize, string txtSearch, string accountId);
    }
}
