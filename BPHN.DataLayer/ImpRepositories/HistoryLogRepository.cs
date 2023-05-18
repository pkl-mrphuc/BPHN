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
        public bool Write(HistoryLog history)
        {
            return true;
        }
    }
}
