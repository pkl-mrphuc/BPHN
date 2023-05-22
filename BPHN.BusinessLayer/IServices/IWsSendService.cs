using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPHN.BusinessLayer.IServices
{
    public interface IWsSendService
    {
        Task SERVER_AfterClientLoginSuccess(string accountId, string connectionId);
    }
}
