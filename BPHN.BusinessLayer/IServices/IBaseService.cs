﻿using BPHN.ModelLayer;

namespace BPHN.BusinessLayer.IServices
{
    public interface IBaseService
    {
        bool ValidateModelByAttribute(object model, List<string> ignoreProperties);
        string BuildLinkDescription(Guid historyLogId);
        Task<bool> IsValidPermission(Guid accountId, FunctionTypeEnum functionType);
    }
}
