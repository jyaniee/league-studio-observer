namespace LeagueStudio.Observer.ViewModels;

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using LeagueStudio.Observer.Models;
using LeagueStudio.Observer.Services.Server;
using System.ComponentModel.DataAnnotations;
using System.Windows.Threading;
public partial class MainViewModel : ObservableValidator
{
    private readonly IObserverApiClient _observerApiClient;
    private readonly DispatcherTimer _connectionTimer;

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

        _connectionTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(2)
        };

        _connectionTimer.Tick += ConnectionTimer_Tick;
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ApplyMatchInfoCommand))]
    private bool hasCheckedConnection;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ApplyMatchInfoCommand))]
    private bool isServerConnected;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Match ID를 입력해주세요.")]
    private string matchId = "test-match-001";

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Observer ID를 입력해주세요.")]
    private string observerId = "observer-01";

    [ObservableProperty]
    private DragonType selectedDragon = DragonType.Unknown;

    [ObservableProperty]
    private string connectionStatus = "Disconnected";

    [ObservableProperty]
    private string resultMessage = "먼저 Check를 눌러 서버 연결을 확인해주세요.";

    [RelayCommand]
    private async Task CheckConnectionAsync()
    {
        HasCheckedConnection = true;

        bool connected =
            await _observerApiClient.CheckConnectionAsync();

        IsServerConnected = connected;

        ConnectionStatus =
            connected ? "Connected" : "Disconnected";

        ResultMessage =
            connected
                ? "입력한 경기 정보가 Server에 등록됩니다."
                : "Server에 연결할 수 없습니다. 연결 상태를 확인해주세요.";

        if (!_connectionTimer.IsEnabled)
        {
            _connectionTimer.Start();
        }
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

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "대회 이름을 입력해주세요.")]
    private string tournamentName = string.Empty;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "세트 번호를 입력해주세요.")]
    private int? setNumber = 1;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Blue 팀 이름을 입력해주세요.")]
    private string blueTeamName = string.Empty;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Blue 팀 태그를 입력해주세요.")]
    private string blueTeamTag = string.Empty;

    [ObservableProperty]
    private string blueTeamLogoUrl = string.Empty;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Red 팀 이름을 입력해주세요.")]
    private string redTeamName = string.Empty;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Red 팀 태그를 입력해주세요.")]
    private string redTeamTag = string.Empty;

    [ObservableProperty]
    private string redTeamLogoUrl = string.Empty;

    [RelayCommand(CanExecute = nameof(CanApplyMatchInfo))]
    private async Task ApplyMatchInfoAsync()
    {
        if (!HasCheckedConnection)
        {
            ResultMessage = "먼저 Check를 눌러 서버 연결을 확인해주세요.";
            return;
        }

        if (!IsServerConnected)
        {
            ResultMessage = "Connected 상태에서만 경기 정보를 적용할 수 있습니다.";
            return;
        }

        ValidateAllProperties(); // ObservableValidator가 제공하는 메서드(ViewModel 안의 Validation 규칙들 전부 검사)
        if (HasErrors)
        {
            ResultMessage = "필수 입력 항목을 확인해주세요.";
            return;
        }

        var request = new ObserverMatchInfoRequest
        {
            MatchId = MatchId,
            ObserverId = ObserverId,
            SentAt = DateTimeOffset.UtcNow.ToString("O"),

            TournamentName = TournamentName,

            SetNumber = SetNumber,

            Teams = new ObserverTeams
            {
                Blue = new TeamInfo
                {
                    Name = BlueTeamName,
                    Tag = BlueTeamTag,
                    LogoUrl = string.IsNullOrWhiteSpace(BlueTeamLogoUrl) ? null : BlueTeamLogoUrl
                },

                Red = new TeamInfo
                {
                    Name = RedTeamName,
                    Tag = RedTeamTag,
                    LogoUrl = string.IsNullOrWhiteSpace(RedTeamLogoUrl) ? null : RedTeamLogoUrl
                }
            }
        };

        try
        {
            await _observerApiClient.SendMatchInfoAsync(request);

            ResultMessage = "경기 정보가 적용되었습니다.";
        }
        catch (Exception ex)
        {
            ResultMessage = $"경기 정보 적용 실패: {ex.Message}";
        }
    }

    private bool CanApplyMatchInfo()
    {
        return HasCheckedConnection && IsServerConnected;
    }


    private void UpdateConnectionState(bool connected)
    {
        bool connectionChanged =
            IsServerConnected != connected;

        IsServerConnected = connected;

        ConnectionStatus =
            connected ? "Connected" : "Disconnected";

        if (!connectionChanged)
        {
            return;
        }

        ResultMessage =
            connected
                ? "Server에 연결되었습니다. 경기 정보를 전송할 수 있습니다."
                : "Server 연결이 끊어졌습니다. 연결 상태를 확인해주세요.";
    }

    private async void ConnectionTimer_Tick(
        object? sender,
        EventArgs e)
    {
        bool connected =
            await _observerApiClient.CheckConnectionAsync();

        UpdateConnectionState(connected);
    }
}
