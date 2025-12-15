using Breakthrough.ConsoleApp.Common;

namespace Breakthrough.ConsoleApp.Domain.Rules;

/// <summary>
/// Стандартные правила игры "Прорыв".
/// </summary>
public sealed class BreakthroughRules : IBreakthroughRules
{
    /// <summary>
    /// Возвращает направление движения пешек для стороны (-1 для белых, +1 для черных).
    /// </summary>
    /// <param name="side"></param>
    /// <returns></returns>
    public int ForwardDirection(Side side) => side == Side.White ? -1 : +1;

    /// <summary>
    /// Проверяет, является ли ряд победным для стороны.
    /// </summary>
    /// <param name="side"></param>
    /// <param name="row"></param>
    /// <returns></returns>
    public bool IsWinningRow(Side side, int row)
        => side == Side.White ? row == 0 : row == 7;
}
