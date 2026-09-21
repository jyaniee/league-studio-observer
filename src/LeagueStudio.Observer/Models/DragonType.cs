namespace LeagueStudio.Observer.Models;

public enum DragonType
{
    Unknown,
    Cloud,
    Infernal,
    Mountain,
    Ocean,
    Hextech,
    Chemtech,
    Elder
}

public static class DragonTypeExtensions
{
    public static string ToApiValue(this DragonType dragonType)
    {
        return dragonType switch
        {
            DragonType.Cloud => "cloud",
            DragonType.Infernal => "infernal",
            DragonType.Mountain => "mountain",
            DragonType.Ocean => "ocean",
            DragonType.Hextech => "hextech",
            DragonType.Chemtech => "chemtech",
            DragonType.Elder => "elder",
            _ => throw new ArgumentOutOfRangeException(
                nameof(dragonType),
                dragonType,
                "Unknown dragon type cannot be sent."
            )
        };
    }
}