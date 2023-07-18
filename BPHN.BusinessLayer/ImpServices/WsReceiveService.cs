﻿using BPHN.BusinessLayer.IServices;
using Microsoft.AspNetCore.SignalR;

namespace BPHN.BusinessLayer.ImpServices
{
    public class WsReceiveService : Hub<IWsSendService>
    {
        private readonly static ConnectionMapping<string> _connections =
            new ConnectionMapping<string>();

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

        public void PushNotification(List<string> accountIds, string currentId, int type)
        {
            for (int i = 0; i < accountIds.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(currentId) && !currentId.Equals(accountIds[i]))
                {
                    var connections = _connections.GetConnections(accountIds[i]);
                    for (int j = 0; j < connections.Count(); j++)
                    {
                        Clients.Client(connections.ElementAt(j)).PushNotification(type);
                    }
                }
            }
            
        }
    }

    public class ConnectionMapping<T>
    {
        private readonly Dictionary<T, HashSet<string>> _connections =
            new Dictionary<T, HashSet<string>>();

        public int Count
        {
            get
            {
                return _connections.Count;
            }
        }

        public void Add(T key, string connectionId)
        {
            lock (_connections)
            {
                HashSet<string> connections;
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

        public IEnumerable<string> GetConnections(T key)
        {
            HashSet<string> connections;
            if (_connections.TryGetValue(key, out connections))
            {
                return connections;
            }

            return Enumerable.Empty<string>();
        }

        public void Remove(T key, string connectionId)
        {
            lock (_connections)
            {
                HashSet<string> connections;
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
