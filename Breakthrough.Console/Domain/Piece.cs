using Breakthrough.ConsoleApp.Common;

namespace Breakthrough.ConsoleApp.Domain;

/// <summary>
/// Базовый класс для игровой фигуры.
/// </summary>
public abstract class Piece
{
    public Side Side { get; }

    protected Piece(Side side) => Side = side;

    /// <summary>Символ для отображения в консоли.</summary>.
    public abstract char Symbol { get; }
}
