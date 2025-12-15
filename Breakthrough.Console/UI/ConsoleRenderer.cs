using Breakthrough.ConsoleApp.Common;
using Breakthrough.ConsoleApp.Domain;

namespace Breakthrough.ConsoleApp.UI;

/// <summary>
/// Отвечает за отрисовку игрового поля и информации в консоли.
/// </summary>
public sealed class ConsoleRenderer
{
    /// <summary>
    /// Рисует доску, статус игры и подсказки.
    /// </summary>
    /// <param name="game"></param>
    public void Render(Game game)
    {
        Console.Clear();

        Console.WriteLine("Breakthrough (Прорыв)");
        Console.WriteLine($"Ход: {game.CurrentPlayer.Name} ({game.Turn})   |   Ходов: {game.PlyCount}");
        Console.WriteLine("Команды: :menu, :exit, :help, :moves, :save [файл], :load [файл], :scores");
        Console.WriteLine("Ход: например  a2 a3");
        Console.WriteLine();

        Console.Write("   ");
        for (char f = 'a'; f <= 'h'; f++) Console.Write($" {f} ");
        Console.WriteLine();

        for (int row = 0; row < 8; row++)
        {
            int rank = 8 - row;
            Console.Write($" {rank} ");

            for (int col = 0; col < 8; col++)
            {
                var piece = game.Board.GetAt(new Position(row, col));
                if (piece is null)
                {
                    Console.Write(" . ");
                }
                else
                {
                    var old = Console.ForegroundColor;
                    Console.ForegroundColor = piece.Side == Side.White ? ConsoleColor.Cyan : ConsoleColor.Yellow;
                    Console.Write($" {piece.Symbol} ");
                    Console.ForegroundColor = old;
                }
            }

            Console.WriteLine($" {rank}");
        }

        Console.Write("   ");
        for (char f = 'a'; f <= 'h'; f++) Console.Write($" {f} ");
        Console.WriteLine();
        Console.WriteLine();
    }

    /// <summary>
    /// Выводит справку по управлению и правилам.
    /// </summary>
    public void PrintHelp()
    {
        Console.WriteLine("Правила (кратко):");
        Console.WriteLine("- Фишки ходят на 1 клетку вперёд или по диагонали (только в пустую).");
        Console.WriteLine("- Бьют на 1 клетку по диагонали вперёд (только если там фигура соперника).");
        Console.WriteLine("- Победа: провести любую фишку на противоположную сторону доски.");
        Console.WriteLine();
        Console.WriteLine("Ввод хода: a2 a3");
        Console.WriteLine("Команды:");
        Console.WriteLine("- :menu");
        Console.WriteLine("- :exit");
        Console.WriteLine("- :help");
        Console.WriteLine("- :moves");
        Console.WriteLine("- :save [file.json] (по умолчанию save.json)");
        Console.WriteLine("- :load [file.json] (по умолчанию save.json)");
        Console.WriteLine("- :scores (таблица рекордов)");
        Console.WriteLine();
    }
}
