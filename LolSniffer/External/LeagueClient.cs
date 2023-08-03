using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LolSniffer
{
    public class LeagueClient
    {
        public int Pid { get; set; }

        public ClientAuthInfo ClientAuthInfo { get; set; }

        public Process Process { get; set; }

        internal LeagueClient(int pid, ClientAuthInfo info, Process process)
        {
            Pid = pid;
            ClientAuthInfo = info;
            Process = process;
        }
    }
}
