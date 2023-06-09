using BPHN.ModelLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPHN.BusinessLayer.IServices
{
    public interface IConfigService
    {
        Task<ServiceResultModel> GetConfigs(string key = null);
        Task<ServiceResultModel> Save(List<Config> configs);
    }
}
