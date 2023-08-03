using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace LolSniffer
{
    public abstract class PlatformBase
    {
        protected abstract string FileName { get; }

        protected abstract string AuthTokenRegex { get; }

        protected abstract string PortRegex { get; }

        protected abstract string GetCommand(int pid);

        public ClientAuthInfo? ExtractArguments(int pid)
        {
            string command = GetCommand(pid);
            string commandLine = GetCommandLine(FileName, command);
            if (!string.IsNullOrEmpty(commandLine))
            {
                return ExtractArguments(commandLine);
            }

            return null;
        }

        public ClientAuthInfo? ExtractArguments(string commandLine)
        {
            MatchCollection matchCollection = Regex.Matches(commandLine, AuthTokenRegex);
            MatchCollection matchCollection2 = Regex.Matches(commandLine, PortRegex);
            ClientAuthInfo clientAuthInfo = new ClientAuthInfo();
            foreach (Match item in matchCollection)
            {
                if (item.Groups.Count != 4)
                {
                    continue;
                }

                string value = item.Groups[1].Value;
                if (!(value == "riotclient"))
                {
                    if (value == "remoting")
                    {
                        clientAuthInfo.RemotingAuthToken = item.Groups[2].Value;
                    }
                }
                else
                {
                    clientAuthInfo.RiotClientAuthToken = item.Groups[2].Value;
                }
            }

            foreach (Match item2 in matchCollection2)
            {
                if (item2.Groups.Count != 4)
                {
                    continue;
                }

                string value = item2.Groups[1].Value;
                if (!(value == "riotclient-"))
                {
                    if (value != null && value.Length == 0)
                    {
                        clientAuthInfo.RemotingPort = Convert.ToInt32(item2.Groups[2].Value);
                    }
                }
                else
                {
                    clientAuthInfo.RiotClientPort = Convert.ToInt32(item2.Groups[2].Value);
                }
            }

            return clientAuthInfo;
        }

        private string? GetCommandLine(string fileName, string command)
        {
            string result = string.Empty;
            try
            {
                Process? process = Process.Start(new ProcessStartInfo
                {
                    Verb = "runas",
                    FileName = fileName,
                    Arguments = command,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = false
                });
                result = process?.StandardOutput.ReadToEnd();
                process?.WaitForExit(5000);
                return result;
            }
            catch (Exception)
            {
                return result;
            }
        }
    }
}