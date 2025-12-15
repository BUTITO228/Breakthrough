namespace Breakthrough.ConsoleApp.Persistence.Models;

public sealed class ScoreboardDto
{
    public List<ScoreEntryDto> Entries { get; set; } = new();
}
