using Breakthrough.ConsoleApp.Common;

namespace Breakthrough.ConsoleApp.Domain.Rules;

public interface IBreakthroughRules
{
    int ForwardDirection(Side side); // White: -1, Black: +1.
    bool IsWinningRow(Side side, int row);
}
