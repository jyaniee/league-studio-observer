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
                new ObserverObjectivesPatch(
                    new ObserverDragonPatch(
                        dragonType.ToApiValue()
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
}