using LolSniffer;

public class WindowsPlatform : PlatformBase
{
    protected override string FileName => "cmd.exe";

    protected override string AuthTokenRegex => "--(riotclient|remoting)-auth-token=(.*?)( --|\n|$|\")";

    protected override string PortRegex => "--(riotclient-|)app-port=(.*?)( --|\n|$|\")";

    protected override string GetCommand(int pid)
    {
        return $"/C \"WMIC PROCESS WHERE ProcessId={pid} GET commandline\"";
    }
}