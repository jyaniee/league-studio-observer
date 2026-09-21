namespace LeagueStudio.Observer.ViewModels;

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using LeagueStudio.Observer.Models;
using LeagueStudio.Observer.Services.Server;
using System.ComponentModel.DataAnnotations;
public partial class MainViewModel : ObservableValidator
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
    private string resultMessage = "입력한 경기 정보가 Server에 등록됩니다.";

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

    [RelayCommand]
    private async Task ApplyMatchInfoAsync()
    {
        ValidateAllProperties(); // ObservableValidator가 제공하는 메서드(ViewModel 안의 Validation 규칙들 전부 검사)
        if (HasErrors)
        {
            ResultMessage = "필수 입력 항목을 확인해주세요.";
            return;
        }

        if (string.IsNullOrWhiteSpace(MatchId))
        {
            ResultMessage = "Match ID를 입력해주세요.";
            return;
        }

        if (string.IsNullOrWhiteSpace(ObserverId))
        {
            ResultMessage = "Observer ID를 입력해주세요.";
            return;
        }

        var request = new ObserverMatchInfoRequest
        {
            MatchId = MatchId,
            ObserverId = ObserverId,
            SentAt = DateTimeOffset.UtcNow.ToString("O"),

            TournamentName =
                string.IsNullOrWhiteSpace(TournamentName)
                    ? null : TournamentName,

            SetNumber = SetNumber,

            Teams = new ObserverTeams
            {
                Blue = new TeamInfo
                {
                    Name = BlueTeamName,
                    Tag = string.IsNullOrWhiteSpace(BlueTeamTag) ? null : BlueTeamTag,
                    LogoUrl = string.IsNullOrWhiteSpace(BlueTeamLogoUrl) ? null : BlueTeamLogoUrl
                },

                Red = new TeamInfo
                {
                    Name = RedTeamName,
                    Tag = string.IsNullOrWhiteSpace(RedTeamTag) ? null : RedTeamTag,
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

}
