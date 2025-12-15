using Breakthrough.ConsoleApp.Common;
using Breakthrough.ConsoleApp.Domain;

namespace Breakthrough.ConsoleApp.UI;

/// <summary>
/// Утилита для чтения и парсинга ввода пользователя.
/// </summary>
public sealed class InputReader
{
    /// <summary>
    /// Считывает строку из консоли и удаляет лишние пробелы.
    /// </summary>
    /// <returns></returns>
    public string ReadLineTrimmed()
        => (Console.ReadLine() ?? string.Empty).Trim();

    /// <summary>
    /// Пытается распознать команду хода (например, "a2 a3").
    /// </summary>
    /// <param name="line"></param>
    /// <param name="move"></param>
    /// <param name="error"></param>
    /// <returns></returns>
    public bool TryReadMove(string line, out Move move, out string error)
    {
        move = default;
        error = string.Empty;

        // Разбиваем строку по пробелам.
        var parts = (line ?? string.Empty)
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (parts.Length != 2)
        {
            error = "Нужно 2 координаты: например  a2 a3";
            return false;
        }

        if (!Position.TryParse(parts[0], out var from))
        {
            error = $"Не понял координату: {parts[0]} (пример: a2)";
            return false;
        }

        if (!Position.TryParse(parts[1], out var to))
        {
            error = $"Не понял координату: {parts[1]} (пример: a3)";
            return false;
        }

        move = new Move(from, to);
        return true;
    }
}
