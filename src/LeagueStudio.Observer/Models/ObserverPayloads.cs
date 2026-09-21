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
    ObserverObjectivesPatch Objectives
);

public sealed record ObserverObjectivesPatch(
    ObserverDragonPatch Dragon
);

public sealed record ObserverDragonPatch(
    string NextDragonType
);