using Breakthrough.ConsoleApp.Common;

namespace Breakthrough.ConsoleApp.Domain;

/// <summary>
/// Информация об игроке.
/// </summary>
public sealed class Player
{
    public string Name { get; }
    public Side Side { get; }

    public Player(string name, Side side)
    {
        Name = string.IsNullOrWhiteSpace(name) ? side.ToString() : name.Trim();
        Side = side;
    }

    public override string ToString() => $"{Name} ({Side})";
}
