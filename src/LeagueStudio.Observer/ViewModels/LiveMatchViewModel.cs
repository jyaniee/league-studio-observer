namespace LeagueStudio.Observer.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LeagueStudio.Observer.Models;
using LeagueStudio.Observer.Services.Server;

public partial class LiveMatchViewModel : ObservableObject
{
    private readonly IObserverApiClient _observerApiClient;

    public LiveMatchViewModel(
        IObserverApiClient observerApiClient)
    {
        _observerApiClient = observerApiClient;
    }

    [ObservableProperty]
    private ObserverState? currentState;

    [ObservableProperty]
    private bool hasMatch;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RefreshCommand))]
    private bool isLoading;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RefreshCommand))]
    private bool canRequestServer;

    [ObservableProperty]
    private string statusMessage =
        "Live Match를 조회해주세요.";

    [RelayCommand(CanExecute = nameof(CanRefresh))]
    private async Task RefreshAsync()
    {
        IsLoading = true;
        StatusMessage = "현재 경기 정보를 불러오는 중입니다.";

        try
        {
            ObserverStateResponse? response =
                await _observerApiClient.GetObserverStateAsync();

            if (response is null ||
                !response.Ok ||
                string.IsNullOrWhiteSpace(response.State.MatchId))
            {
                CurrentState = null;
                HasMatch = false;
                StatusMessage =
                    "현재 등록된 경기 정보가 없습니다.";

                return;
            }

            CurrentState = response.State;
            HasMatch = true;
            StatusMessage =
                "현재 경기 정보를 불러왔습니다.";
        }
        catch (Exception)
        {
            CurrentState = null;
            HasMatch = false;
            StatusMessage =
                "Server에 연결되어 있지 않습니다.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private bool CanRefresh()
    {
        return !IsLoading && CanRequestServer;
    }

    public void SetServerAvailability(bool available, string? unavailableMessage = null)
    {
        bool availabilityChanged =
            CanRequestServer != available;

        CanRequestServer = available;

        if (available)
        {
            if (availabilityChanged)
            {
                StatusMessage =
                    "Server에 연결되었습니다. Refresh를 눌러 현재 경기 정보를 확인해주세요.";
            }

            return;
        }

        CurrentState = null;
        HasMatch = false;

        StatusMessage = unavailableMessage ?? "Server에 연결되어 있지 않습니다.";
    }
}