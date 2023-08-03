using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace LolSniffer.Services
{
    public class LobbyService
    {
        private List<LobbyHandler> _handlers = new List<LobbyHandler>();
        private bool _update = true;
        private ILogger _logger;
        private HashSet<string> _uniqueNames = new HashSet<string>();
        public delegate void NameListUpdatedEventHandler(object source, EventArgs args);
        public event NameListUpdatedEventHandler NameListUpdated;

        public LobbyService(ILogger<LobbyService> logger)
        {
            _logger = logger;
        }

        public void StartService()
        {
            Debug.WriteLine("Starting service...");
            var watcher = new LeagueClientWatcher();

            watcher.OnLeagueClientOpen += (clientWatcher, client) =>
            {
                var handler = new LobbyHandler(new LeagueApi(client.ClientAuthInfo.RiotClientAuthToken, client.ClientAuthInfo.RiotClientPort));
                _handlers.Add(handler);
                handler.OnUpdate += (lobbyHandler, names) => { _update = true; };
                handler.Start();
                _update = true;
            };

            new Thread(async () => { await watcher.Observe(); })
            {
                IsBackground = true
            }.Start();

            new Thread(() => { Refresh(); })
            {
                IsBackground = true
            }.Start();
        }

        protected virtual void OnNameListUpdated()
        {
            NameListUpdated?.Invoke(this, EventArgs.Empty);
        }

        private void Refresh()
        {
            while (true)
            {
                if (_update && _handlers.Count > 0)
                {
                    if (_handlers[0].GetSummoners() != null && _handlers[0].GetSummoners().Length == 5)
                    {
                        for (int i = 0; i < _handlers.Count; i++)
                        {
                            IList<string> names = _handlers[i].GetSummoners();
                            foreach (string name in names)
                            {
                                _uniqueNames.Add(name);  // Add the name to the in-memory list
                            }
                        }
                        _update = false;
                        OnNameListUpdated();  // Notify that name list has been updated
                    }
                }
                Thread.Sleep(2000); // Sleep for 2 seconds before checking again
            }
        }
        public IList<string> GetUniqueNames()
        {
            return _uniqueNames.ToList();
        }
    }
}
