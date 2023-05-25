using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPHN.DataLayer.ImpRepositories
{
    public class ConfigRepository : IConfigRepository
    {
        public List<Config> GetConfigs(Guid accountId, string key = null)
        {
            return new List<Config>()
            {
                new Config()
                {
                    Key = "DarkMode",
                    Value = true
                },
                new Config()
                {
                    Key = "Language",
                    Value = "vn"
                }
            };
        }
    }
}
