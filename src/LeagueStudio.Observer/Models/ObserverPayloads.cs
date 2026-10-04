namespace LeagueStudio.Observer.Models;

public sealed record ObserverStatePatchPayload(
    string MatchId,
    string ObserverId,
    string SentAt,
    string ObservedAt,
    ObserverStatePatch Patch,
    Dictionary<string, double> Confidence
);

public sealed record ObserverStatePatch(
    ObserverTeamsPatch? Teams = null,
    ObserverObjectivesPatch? Objectives = null
);

public sealed record ObserverTeamsPatch(
    ObserverTeamPatch? Blue = null,
    ObserverTeamPatch? Red = null
);

public sealed record ObserverTeamPatch(
    double? GlobalGold = null
);

public sealed record ObserverObjectivesPatch(
    ObserverDragonPatch? Dragon = null
);

public sealed record ObserverDragonPatch(
    string? NextDragonType = null
);