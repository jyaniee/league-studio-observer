namespace LeagueStudio.Observer.Models;

public sealed class ObserverTeams
{
	public TeamInfo Blue { get; set; } = new();

	public TeamInfo Red { get; set; } = new();
}