namespace LeagueStudio.Observer.Models;

public sealed class ObserverMatchInfoRequest
{
    public string MatchId { get; set; } = string.Empty;

    public string ObserverId { get; set; } = string.Empty;

    public string SentAt { get; set; } = string.Empty;

    public string? TournamentName { get; set; }
    
    public int? SetNumber { get; set; }

    public ObserverTeams Teams { get; set; } = new();
}