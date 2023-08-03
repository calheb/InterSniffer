using LolSniffer.Services;
using System.ComponentModel;
using System.Windows.Input;

namespace LolSniffer;
public class SummonerViewModel : INotifyPropertyChanged
{
    private string _Id;
    private string _name;
    private int _profileIconId;
    private int _summonerLevel;
    private string _profileIconUrl;
    private double _winRate;
    private int _wins;
    private int _losses;
    private IList<string> _summoners;


    private LobbyService _lobbyService;

    public SummonerViewModel(LobbyService lobbyService)
    {
        _lobbyService = lobbyService;
        _lobbyService.StartService();
    }

    public string Id
    {
        get { return _Id; }
        set
        {
            _Id = value;
            OnPropertyChanged(nameof(Id));
        }
    }

    public string Name
    {
        get { return _name; }
        set
        {
            _name = value;
            OnPropertyChanged(nameof(Name));
        }
    }

    public int ProfileIconId
    {
        get { return _profileIconId; }
        set
        {
            _profileIconId = value;
            OnPropertyChanged(nameof(ProfileIconId));
        }
    }

    public int SummonerLevel
    {
        get { return _summonerLevel; }
        set
        {
            _summonerLevel = value;
            OnPropertyChanged(nameof(SummonerLevel));
        }
    }

    public string ProfileIconUrl
    {
        get { return _profileIconUrl; }
        set
        {
            if (_profileIconUrl != value)
            {
                _profileIconUrl = value;
                OnPropertyChanged(nameof(ProfileIconUrl));
            }
        }
    }
    public double WinRate
    {
        get => _winRate;
        set
        {
            _winRate = value;
            OnPropertyChanged(nameof(WinRate));
        }
    }

    public int Wins
    {
        get => _wins;
        set
        {
            _wins = value;
            OnPropertyChanged(nameof(Wins));
        }
    }
    public int Losses
    {
        get => _losses;
        set
        {
            _losses = value;
            OnPropertyChanged(nameof(Losses));
        }
    }

    public IList<string> Summoners
    {
        get => _summoners;
        set
        {
            _summoners = value;
            OnPropertyChanged(nameof(Summoners));
        }
    }

    public ICommand OpenUrlCommand { get; set; }

    public SummonerViewModel()
    {
        OpenUrlCommand = new Command(async () => await OpenUrl());
    }

    async Task OpenUrl()
    {
        string formattedName = Name.Replace(" ", "+");
        await Launcher.OpenAsync(new Uri($"https://na.op.gg/summoner/userName={formattedName}"));
    }


    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
