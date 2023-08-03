using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace LolSniffer
{
    public class LeagueClientWatcher
    {
        public delegate void OnLeagueClient(LeagueClientWatcher watcher, LeagueClient client);

        private readonly PlatformBase _platform;

        public List<LeagueClient> Clients { get; }

        public event OnLeagueClient? OnLeagueClientOpen;

        public event OnLeagueClient? OnLeagueClientExit;

        public LeagueClientWatcher()
        {
            //_platform = (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? new WindowsPlatform() : (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? ((PlatformBase)new OsxPlatform()) : ((PlatformBase)new LinuxPlatform())));
            _platform = new WindowsPlatform();
            Clients = new List<LeagueClient>();
        }

        public async Task Observe(CancellationToken token = default(CancellationToken))
        {
            while (!token.IsCancellationRequested)
            {
                foreach (Process process in Process.GetProcesses().Where(delegate (Process p)
                {
                    string processName = p.ProcessName;
                    return processName == "LeagueClientUx" || processName == "LeagueClientUx.exe";
                }))
                {
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }

                    if (Clients.Any((LeagueClient d) => d.Pid == process.Id))
                    {
                        continue;
                    }

                    ClientAuthInfo clientAuthInfo = _platform.ExtractArguments(process.Id);
                    if (clientAuthInfo != null)
                    {
                        LeagueClient client = new LeagueClient(process.Id, clientAuthInfo, process);
                        process.EnableRaisingEvents = true;
                        process.Exited += delegate
                        {
                            Clients.Remove(client);
                            OnLeagueClientExitInvoke(client);
                        };
                        Clients.Add(client);
                        OnLeagueClientInvoke(client);
                    }
                }

                Thread.Sleep(1000);
            }
        }

        private void OnLeagueClientExitInvoke(LeagueClient client)
        {
            this.OnLeagueClientExit?.Invoke(this, client);
        }

        private void OnLeagueClientInvoke(LeagueClient client)
        {
            this.OnLeagueClientOpen?.Invoke(this, client);
        }
    }
}