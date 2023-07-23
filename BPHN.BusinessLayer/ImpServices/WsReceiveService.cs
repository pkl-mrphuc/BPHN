using BPHN.BusinessLayer.IServices;
using Microsoft.AspNetCore.SignalR;

namespace BPHN.BusinessLayer.ImpServices
{
    public class WsReceiveService : Hub<IWsSendService>
    {
        private readonly static ConnectionMapping _connections = new ConnectionMapping();

        public void AddConnection(string accountId)
        {
            var connections = _connections.GetConnections(accountId);
            if(connections.Count() > 0)
            {
                for (int i = 0; i < connections.Count(); i++)
                {
                    _connections.Remove(accountId, connections.ElementAt(i));
                }
            }
            _connections.Add(accountId, Context.ConnectionId);
        }

        public void PushNotification(List<string> accountIds, string currentId, int type, string model)
        {
            for (int i = 0; i < accountIds.Count; i++)
            {
                var connections = _connections.GetConnections(accountIds[i]);
                for (int j = 0; j < connections.Count(); j++)
                {
                    Clients.Client(connections.ElementAt(j)).PushNotification(type, model);
                }
            }
            
        }
    }

    public class ConnectionMapping
    {
        private readonly Dictionary<string, HashSet<string>> _connections =
            new Dictionary<string, HashSet<string>>();

        public int Count
        {
            get
            {
                return _connections.Count;
            }
        }

        public void Add(string key, string connectionId)
        {
            lock (_connections)
            {
                HashSet<string>? connections;
                if (!_connections.TryGetValue(key, out connections))
                {
                    connections = new HashSet<string>();
                    _connections.Add(key, connections);
                }

                lock (connections)
                {
                    connections.Add(connectionId);
                }
            }
        }

        public IEnumerable<string> GetConnections(string key)
        {
            HashSet<string>? connections;
            if (_connections.TryGetValue(key, out connections))
            {
                return connections;
            }

            return Enumerable.Empty<string>();
        }

        public void Remove(string key, string connectionId)
        {
            lock (_connections)
            {
                HashSet<string>? connections;
                if (!_connections.TryGetValue(key, out connections))
                {
                    return;
                }

                lock (connections)
                {
                    connections.Remove(connectionId);

                    if (connections.Count == 0)
                    {
                        _connections.Remove(key);
                    }
                }
            }
        }
    }
}
