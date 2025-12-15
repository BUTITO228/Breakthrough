using Breakthrough.ConsoleApp.Common;

namespace Breakthrough.ConsoleApp.Persistence.Models;

public sealed class ScoreEntryDto
{
    public string WinnerName { get; set; } = "";
    public Side WinnerSide { get; set; } = Side.White;

    public string LoserName { get; set; } = "";
    public int PlyCount { get; set; }        // сколько ходов сделано (по полходам).
    public DateTime DateUtc { get; set; }    // когда закончилась игра (UTC).
}
