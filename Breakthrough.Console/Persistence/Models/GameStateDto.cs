using Breakthrough.ConsoleApp.Common;

namespace Breakthrough.ConsoleApp.Persistence.Models;

/// <summary>
/// DTO для сериализации состояния игры.
/// </summary>
public sealed class GameStateDto
{
    public string WhiteName { get; set; } = "White";
    public string BlackName { get; set; } = "Black";
    public Side Turn { get; set; } = Side.White;

    public int PlyCount { get; set; } = 0;

    /// <summary>
    /// Массив строк, представляющий доску (8 строк по 8 символов).
    /// </summary>
    public string[] Rows { get; set; } = new string[8];
}
