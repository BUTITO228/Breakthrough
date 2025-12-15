using Breakthrough.ConsoleApp.Common;

namespace Breakthrough.ConsoleApp.Domain;

/// <summary>
/// Игровой ход: перемещение фишки из клетки <see cref="From"/> в клетку <see cref="To"/>.
/// </summary>
/// <param name="From">Начальная позиция (откуда ходим).</param>
/// <param name="To">Конечная позиция (куда ходим).</param>
public readonly record struct Move(Position From, Position To);
