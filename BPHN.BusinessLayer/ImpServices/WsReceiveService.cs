using BPHN.BusinessLayer.IServices;
using Microsoft.AspNetCore.SignalR;

namespace BPHN.BusinessLayer.ImpServices
{
    public class WsReceiveService : Hub<IWsSendService>
    {
        
        
    }
}
