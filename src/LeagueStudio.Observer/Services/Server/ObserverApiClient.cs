using System.Net.Http;
using System.Net.Http.Json;
using LeagueStudio.Observer.Models;

namespace LeagueStudio.Observer.Services.Server;

public sealed class ObserverApiClient : IObserverApiClient
{
    private readonly HttpClient _httpClient;

    public ObserverApiClient()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:3002")
        };
    }

    public async Task<bool> CheckConnectionAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var response =
                await _httpClient.GetAsync(
                    "/observer/state",
                    cancellationToken);

            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task SendMatchInfoAsync(
        ObserverMatchInfoRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response =
            await _httpClient.PostAsJsonAsync(
                "/observer/match-info",
                request,
                cancellationToken);

        response.EnsureSuccessStatusCode();
    }

    public async Task<ObserverStateResponse?> GetObserverStateAsync(
    CancellationToken cancellationToken = default)
    {
        using var response =
            await _httpClient.GetAsync(
                "/observer/state",
                cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<ObserverStateResponse>(
            cancellationToken: cancellationToken);
    }

    public async Task ResetObserverStateAsync(
        CancellationToken cancellationToken = default)
    {
        using var response =
            await _httpClient.PostAsync(
                "/observer/reset",
                content: null,
                cancellationToken);

        response.EnsureSuccessStatusCode();
    }

    public async Task SendNextDragonAsync(
        string matchId,
        string observerId,
        DragonType dragonType,
        double confidence,
        CancellationToken cancellationToken = default)
    {
        if (dragonType is DragonType.Unknown or DragonType.Elder)
        {
            throw new ArgumentException(
                "Next dragon must be a normal dragon.",
                nameof(dragonType));
        }

        var now = DateTimeOffset.UtcNow.ToString("O");


        var payload = new ObserverStatePatchPayload(
            MatchId: matchId,
            ObserverId: observerId,
            SentAt: now,
            ObservedAt: now,
            Patch: new ObserverStatePatch(
                Objectives: new ObserverObjectivesPatch(
                    Dragon: new ObserverDragonPatch(
                        NextDragonType: dragonType.ToApiValue()
                    )
                )
            ),
            Confidence: new Dictionary<string, double>
            {
                ["objectives.dragon.nextDragonType"] = confidence
            }
        );

        using var response =
            await _httpClient.PostAsJsonAsync(
                "/observer/state-patch",
                payload,
                cancellationToken);

        response.EnsureSuccessStatusCode();
    }

    public async Task SendTeamGoldAsync(
        string matchId,
        string observerId,
        TeamSide side,
        double globalGold,
        double confidence,
        CancellationToken cancellationToken = default)
    {
        if (globalGold < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(globalGold),
                "Global gold cannot be negative.");
        }

        var now = DateTimeOffset.UtcNow.ToString("O");

        ObserverTeamsPatch teamsPatch =
           side switch
           {
               TeamSide.Blue => new ObserverTeamsPatch(
                   Blue: new ObserverTeamPatch(
                       GlobalGold: globalGold
                   )
               ),

               TeamSide.Red => new ObserverTeamsPatch(
                   Red: new ObserverTeamPatch(
                       GlobalGold: globalGold
                   )
               ),

               _ => throw new ArgumentOutOfRangeException(
                   nameof(side),
                   side,
                   "Unknown team side.")
           };

        string confidenceKey =
            side == TeamSide.Blue ? "teams.blue.globalGold" : "teams.red.globalGold";

        var payload = new ObserverStatePatchPayload(
            MatchId: matchId,
            ObserverId: observerId,
            SentAt: now,
            ObservedAt: now,
            Patch: new ObserverStatePatch(
                Teams: teamsPatch
            ),
            Confidence: new Dictionary<string, double>
            {
                [confidenceKey] = confidence
            }
        );

        using var response =
            await _httpClient.PostAsJsonAsync(
                "/observer/state-patch",
                payload,
                cancellationToken);

        response.EnsureSuccessStatusCode();
    }
}