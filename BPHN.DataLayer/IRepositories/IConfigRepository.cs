using BPHN.ModelLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPHN.DataLayer.IRepositories
{
    public interface IConfigRepository
    {
        List<Config> GetConfigs(Guid accountId, string key = null);
    }
}
