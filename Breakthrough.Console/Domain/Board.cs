using Breakthrough.ConsoleApp.Common;

namespace Breakthrough.ConsoleApp.Domain;

/// <summary>
/// ѕредставл€ет игровое поле 8x8.
/// </summary>
public sealed class Board
{
    private readonly Piece?[,] _cells = new Piece?[8, 8];

    /// <summary>
    /// ¬озвращает фигуру в указанной позиции или null, если клетка пуста.
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public Piece? GetAt(Position p) => p.IsInside() ? _cells[p.Row, p.Col] : null;

    /// <summary>
    /// ”станавливает фигуру в указанную позицию.
    /// </summary>
    /// <param name="p"></param>
    /// <param name="piece"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void SetAt(Position p, Piece? piece)
    {
        if (!p.IsInside()) throw new ArgumentOutOfRangeException(nameof(p));
        _cells[p.Row, p.Col] = piece;
    }

    /// <summary>
    /// —брасывает доску и расставл€ет фигуры в начальную позицию дл€ игры "ѕрорыв".
    /// </summary>
    public void ResetInitialSetup()
    {
        Array.Clear(_cells, 0, _cells.Length);

        // „ерные занимают верхние 2 р€да (р€ды 0 и 1).
        for (int r = 0; r <= 1; r++)
            for (int c = 0; c < 8; c++)
                _cells[r, c] = new Pawn(Side.Black);

        // Ѕелые занимают нижние 2 р€да (р€ды 6 и 7).
        for (int r = 6; r <= 7; r++)
            for (int c = 0; c < 8; c++)
                _cells[r, c] = new Pawn(Side.White);
    }

    /// <summary>
    /// ѕеречисл€ет все фигуры на доске с их позици€ми.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<(Position Pos, Piece Piece)> EnumeratePieces()
    {
        for (int r = 0; r < 8; r++)
        for (int c = 0; c < 8; c++)
        {
            var p = _cells[r, c];
            if (p is not null)
                yield return (new Position(r, c), p);
        }
    }
}
