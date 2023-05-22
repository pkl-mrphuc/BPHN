using BPHN.BusinessLayer.IServices;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPHN.BusinessLayer.ImpServices
{
    public class WsService : Hub<IWsService>
    {
        public async Task SendToUser(string user, string message)
        {
            await Clients.All.SendToUser(user, message);
        }
    }
}
