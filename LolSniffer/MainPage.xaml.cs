using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;
using LolSniffer.Entities;
using LolSniffer.Services;
using Microsoft.Maui.Controls;

namespace LolSniffer
{
    public partial class MainPage : ContentPage, INotifyPropertyChanged
    {
        public ObservableCollection<SummonerViewModel> SummonerViewModels { get; set; }
        public double LobbyWinRate { get; set; }
        private readonly LobbyService _lobbyService;

        public MainPage()
        {
            InitializeComponent();

            // Get the LobbyService instance from the Service Locator
            _lobbyService = (LobbyService)MauiProgram.CreateMauiApp().Services.GetService(typeof(LobbyService));

            // Subscribe to the NameListUpdated event
            _lobbyService.NameListUpdated += OnLobbyServiceNameListUpdated;

            this.SummonerViewModels = new ObservableCollection<SummonerViewModel>();
            this.BindingContext = this;
        }

        private void OnLobbyServiceNameListUpdated(object source, EventArgs args)
        {
            // When the NameListUpdated event is triggered, get the _UniqueNames list from the service
            var uniqueNames = _lobbyService.GetUniqueNames();
            SearchSummoners(uniqueNames);

            // Here you can do whatever you need with the uniqueNames list
            // ...
        }

        private void OnSearchClicked(object sender, EventArgs e)
        {
            //SearchSummoners();
        }

        private async void SearchSummoners(IList<string> names)
        {
            IList<string> summonerNames = names;
            IList<string> summonerNamesTEST = names.Distinct().Take(5).ToList();


            Console.WriteLine("Summoner names: " + String.Join(", ", summonerNames));
            Debug.WriteLine("Summoner names: " + String.Join(", ", summonerNames));
            Console.WriteLine("Summoner names: " + String.Join(", ", summonerNamesTEST));
            Debug.WriteLine("Summoner names: " + String.Join(", ", summonerNamesTEST));

            HttpClient client = new HttpClient();

            string apiKey = "RGAPI-400795c0-4d36-47c5-b699-2626e7f376e4";

            foreach (var summonerName in summonerNames)
            {
                Console.WriteLine("Summoner Name: " + summonerName);
                HttpResponseMessage response = await client.GetAsync($"https://na1.api.riotgames.com/lol/summoner/v4/summoners/by-name/{summonerName}?api_key={apiKey}");
                Console.WriteLine("API response status: " + response.StatusCode);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var summoner = JsonSerializer.Deserialize<Summoner>(json);
                    var summonerViewModel = new SummonerViewModel
                    {
                        Name = summoner.Name,
                        ProfileIconId = summoner.ProfileIconId,
                        SummonerLevel = summoner.SummonerLevel,
                    };
                    string version = "13.14.1";
                    summonerViewModel.ProfileIconUrl = $"http://ddragon.leagueoflegends.com/cdn/{version}/img/profileicon/{summoner.ProfileIconId}.png";

                    this.SummonerViewModels.Add(summonerViewModel);

                    HttpResponseMessage leagueResponse = await client.GetAsync($"https://na1.api.riotgames.com/lol/league/v4/entries/by-summoner/{summoner.Id}?api_key={apiKey}");

                    if (leagueResponse.IsSuccessStatusCode)
                    {
                        string leagueJson = await leagueResponse.Content.ReadAsStringAsync();
                        var leagueEntries = JsonSerializer.Deserialize<List<LeagueEntry>>(leagueJson);

                        foreach (var entry in leagueEntries)
                        {
                            if (entry.QueueType == "RANKED_SOLO_5x5")
                            {
                                summonerViewModel.WinRate = (double)entry.Wins / (double)(entry.Wins + entry.Losses);
                                summonerViewModel.Wins = entry.Wins;
                                summonerViewModel.Losses = entry.Losses;
                                break;
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Request for league entries failed with status code: {leagueResponse.StatusCode}");
                        string content = await leagueResponse.Content.ReadAsStringAsync();
                        Console.WriteLine($"Response content: {content}");
                    }
                }
            }
            int count = 0;
            double winRateTotal = 0;
            foreach (SummonerViewModel summonerViewModel in SummonerViewModels)
            {
                if (summonerViewModel.Wins > 0 || summonerViewModel.Losses > 0)
                {
                    winRateTotal += summonerViewModel.WinRate;
                    count++;
                }
            }
            if (count > 0)
            {
                LobbyWinRate = winRateTotal / count;
                OnPropertyChanged(nameof(LobbyWinRate));
            }

            Device.BeginInvokeOnMainThread(() =>
            {
                loadingIndicator.IsRunning = false;
                loadingIndicator.IsVisible = false;
                loadingText.IsVisible = false;
                SearchBtn.IsVisible = false;
                openAllBtn.IsVisible = true;
                totalWinRate.IsVisible = true;
            });
        }
        private async void OnOpenAllClicked(object sender, EventArgs e)
        {
            foreach (var summoner in SummonerViewModels)
            {
                string formattedName = summoner.Name.Replace(" ", "+");
                await Launcher.OpenAsync(new Uri($"https://na.op.gg/summoner/userName={formattedName}"));
            }
        }
    }
}
