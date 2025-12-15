namespace Breakthrough.ConsoleApp.Common;

/// <summary>
/// Сторона игрока (Белые или Черные).
/// </summary>
public enum Side
{
    White = 0,
    Black = 1
}

/// <summary>
/// Методы расширения для перечисления Side.
/// </summary>
public static class SideExtensions
{
    public static Side Opponent(this Side side) => side == Side.White ? Side.Black : Side.White;
}
