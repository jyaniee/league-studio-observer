using System;
using System.Collections.Generic;
using System.Text;

namespace LeagueStudio.Observer.Models;

public sealed class ObserverStateResponse
{
    public bool Ok { get; set; }

    public ObserverState State { get; set; } = new();
}

public sealed class ObserverState
{
    public string? MatchId { get; set; }

    public string? ObserverId { get; set; }

    public string? TournamentName { get; set; }

    public int? SetNumber { get; set; }

    public ObserverTeams Teams { get; set; } = new();

    public string? UpdatedAt { get; set; }
}
