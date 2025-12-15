using System.Globalization;

namespace Breakthrough.ConsoleApp.Common;

/// <summary>
///  оординаты клетки на доске (строка, столбец).   
/// </summary>
/// <param name="Row"></param>
/// <param name="Col"></param>
public readonly record struct Position(int Row, int Col)
{
    /// <summary>
    /// ѕровер€ет, наход€тс€ ли координаты в пределах доски 8x8.
    /// </summary>
    /// <returns></returns>
    public bool IsInside() => Row is >= 0 and < 8 && Col is >= 0 and < 8;

    /// <summary>
    /// ¬озвращает строковое представление в шахматной нотации (например, "a1").
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        char file = (char)('a' + Col);
        int rank = 8 - Row;
        return $"{file}{rank}";
    }

    /// <summary>
    /// ѕытаетс€ распарсить строку координат (например, "e2") в позицию.
    /// </summary>
    /// <param name="s"></param>
    /// <param name="pos"></param>
    /// <returns></returns>
    public static bool TryParse(string s, out Position pos)
    {
        pos = default;
        s = (s ?? string.Empty).Trim().ToLowerInvariant();
        if (s.Length != 2) return false;

        char file = s[0];
        char rankChar = s[1];

        if (file < 'a' || file > 'h') return false;
        if (rankChar < '1' || rankChar > '8') return false;

        int col = file - 'a';
        int rank = int.Parse(rankChar.ToString(), CultureInfo.InvariantCulture);
        int row = 8 - rank;

        pos = new Position(row, col);
        return pos.IsInside();
    }
}
