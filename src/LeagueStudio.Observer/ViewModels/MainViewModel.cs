namespace LeagueStudio.Observer.ViewModels;

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using LeagueStudio.Observer.Models;
using LeagueStudio.Observer.Services.Server;
public partial class MainViewModel : ObservableObject
{
    private readonly IObserverApiClient _observerApiClient;

    public IReadOnlyList<DragonType> DragonTypes { get; } =
        [
            DragonType.Cloud,
            DragonType.Infernal,
            DragonType.Mountain,
            DragonType.Ocean,
            DragonType.Hextech,
            DragonType.Chemtech
        ];

    public MainViewModel(IObserverApiClient observerApiClient)
    {
        _observerApiClient = observerApiClient;
    }

    [ObservableProperty]
    private string matchId = "test-match-001";

    [ObservableProperty]
    private string observerId = "observer-01";

    [ObservableProperty]
    private DragonType selectedDragon = DragonType.Unknown;

    [ObservableProperty]
    private string connectionStatus = "Disconnected";

    [ObservableProperty]
    private string resultMessage = string.Empty;

    [RelayCommand]
    private async Task CheckConnectionAsync()
    {
        bool connected =
            await _observerApiClient.CheckConnectionAsync();

        ConnectionStatus =
            connected ? "Connected" : "Disconnected";
    }

    [RelayCommand]
    private async Task ApplyDragonAsync()
    {
        if (SelectedDragon == DragonType.Unknown)
        {
            ResultMessage = "드래곤을 선택해주세요.";
            return;
        }
        try
        {
            await _observerApiClient.SendNextDragonAsync(
                MatchId,
                ObserverId,
                SelectedDragon,
                1.0
                );

            ResultMessage =
                $"Applied: {SelectedDragon}";
        }
        catch (Exception ex)
        {
            ResultMessage =
                $"Failed: {ex.Message}";
        }
    }
}
