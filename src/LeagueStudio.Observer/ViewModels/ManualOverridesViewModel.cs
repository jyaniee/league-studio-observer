using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LeagueStudio.Observer.Models;
using LeagueStudio.Observer.Services.Server;
using System.Net.Http;

namespace LeagueStudio.Observer.ViewModels;

public partial class ManualOverridesViewModel : ObservableObject
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

    public ManualOverridesViewModel(
        IObserverApiClient observerApiClient)
    {
        _observerApiClient = observerApiClient;
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ApplyNextDragonCommand))]
    private DragonType selectedDragon = DragonType.Unknown;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ApplyBlueGoldCommand))]
    private int? blueGold;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ApplyRedGoldCommand))]
    private int? redGold;

    [ObservableProperty]
    private string statusMessage =
        "수동으로 보정할 값을 입력해주세요.";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ApplyNextDragonCommand))]
    [NotifyCanExecuteChangedFor(nameof(ApplyBlueGoldCommand))]
    [NotifyCanExecuteChangedFor(nameof(ApplyRedGoldCommand))]
    private bool canRequestServer;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ApplyNextDragonCommand))]
    [NotifyCanExecuteChangedFor(nameof(ApplyBlueGoldCommand))]
    [NotifyCanExecuteChangedFor(nameof(ApplyRedGoldCommand))]
    private bool isLoading;

    public void SetServerAvailability(bool available)
    {
        bool wasAvailable = CanRequestServer;

        CanRequestServer = available;

        if (!available)
        {
            StatusMessage =
                "Server 연결이 필요합니다.";

            return;
        }

        if (!wasAvailable)
        {
            StatusMessage =
                "수동으로 보정할 값을 입력해주세요.";
        }
    }

    [RelayCommand(CanExecute = nameof(CanApplyNextDragon))]
    private async Task ApplyNextDragonAsync()
    {
        IsLoading = true;

        try
        {
            var context =
                await GetCurrentMatchContextAsync();

            if(context is null)
            {
                StatusMessage =
                    "현재 등록된 경기 정보가 없습니다.";

                return;
            }

            await _observerApiClient.SendNextDragonAsync(
                context.Value.MatchId,
                context.Value.ObserverId,
                SelectedDragon,
                1.0);

            StatusMessage =
                $"Next Dragon이 {SelectedDragon}(으)로 적용되었습니다.";
        }
        catch (HttpRequestException)
        {
            StatusMessage =
                "Server에 연결할 수 없습니다.";
        }
        catch (TaskCanceledException)
        {
            StatusMessage =
                "Server 응답 시간이 초과되었습니다.";
        }
        catch (Exception)
        {
            StatusMessage =
                "Next Dragon을 적용하는 중 오류가 발생했습니다.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand(CanExecute = nameof(CanApplyBlueGold))]
    private async Task ApplyBlueGoldAsync()
    {
        if (BlueGold is null)
        {
            return;
        }

        IsLoading = true;

        try
        {
            var context =
                await GetCurrentMatchContextAsync();

            if (context is null)
            {
                StatusMessage =
                    "현재 등록된 경기 정보가 없습니다.";

                return;
            }

            await _observerApiClient.SendTeamGoldAsync(
                context.Value.MatchId,
                context.Value.ObserverId,
                TeamSide.Blue,
                BlueGold.Value,
                1.0);

            StatusMessage =
                $"Blue Team Gold가 {BlueGold.Value:N0}(으)로 적용되었습니다.";
        }
        catch (HttpRequestException)
        {
            StatusMessage =
                "Server에 연결할 수 없습니다.";
        }
        catch (TaskCanceledException)
        {
            StatusMessage =
                "Server 응답 시간이 초과되었습니다.";
        }
        catch (Exception)
        {
            StatusMessage =
                "Blue Team Gold를 적용하는 중 오류가 발생했습니다.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand(CanExecute = nameof(CanApplyRedGold))]
    private async Task ApplyRedGoldAsync()
    {
        if (RedGold is null)
        {
            return;
        }

        IsLoading = true;

        try
        {
            var context =
                await GetCurrentMatchContextAsync();

            if (context is null)
            {
                StatusMessage =
                    "현재 등록된 경기 정보가 없습니다.";

                return;
            }


            await _observerApiClient.SendTeamGoldAsync(
                context.Value.MatchId,
                context.Value.ObserverId,
                TeamSide.Red,
                RedGold.Value,
                1.0);

            StatusMessage =
                $"Red Team Gold가 {RedGold.Value:N0}(으)로 적용되었습니다.";

        }
        catch (HttpRequestException)
        {
            StatusMessage =
                "Server에 연결할 수 없습니다.";
        }
        catch(TaskCanceledException)
        {
            StatusMessage =
                "Server 응답 시간이 초과되었습니다.";
        }
        catch (Exception)
        {
            StatusMessage =
                "Red Team Gold를 적용하는 중 오류가 발생했습니다.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private bool CanApplyNextDragon()
    {
        return CanRequestServer && !IsLoading && SelectedDragon is not DragonType.Unknown and not DragonType.Elder;
    }

    private bool CanApplyBlueGold()
    {
        return CanRequestServer && !IsLoading && BlueGold is >= 0;
    }

    private bool CanApplyRedGold()
    {
        return CanRequestServer && !IsLoading && RedGold is >= 0;
    }

    private async Task<(string MatchId, string ObserverId)?> // Tuple 
        GetCurrentMatchContextAsync()
    {
        var response =
            await _observerApiClient.GetObserverStateAsync();

        if (response is null ||
            !response.Ok ||
            response.State is null ||
            string.IsNullOrWhiteSpace(response.State.MatchId) ||
            string.IsNullOrWhiteSpace(response.State.ObserverId))
        {
            return null;
        }

        return (
            MatchId: response.State.MatchId,
            ObserverId: response.State.ObserverId
        );
    }
}
