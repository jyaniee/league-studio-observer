using LeagueStudio.Observer.Models;

namespace LeagueStudio.Observer.Services.Server;

public interface IObserverApiClient
{
    Task<bool> CheckConnectionAsync(
        CancellationToken cancellationToken = default);

    // 경기정보 전송
    Task SendMatchInfoAsync(
        ObserverMatchInfoRequest request,
        CancellationToken cancellationToken = default);


    Task SendNextDragonAsync(
        string matchId,
        string observerId,
        DragonType dragonType,
        double confidence,
        CancellationToken cancellationToken = default);
}