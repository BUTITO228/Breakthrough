using System.Linq;
using Breakthrough.ConsoleApp.Common;
using Breakthrough.ConsoleApp.Domain;
using Breakthrough.ConsoleApp.Domain.Rules;
using Breakthrough.ConsoleApp.Persistence;
using Breakthrough.ConsoleApp.Persistence.Models;

namespace Breakthrough.ConsoleApp.UI;

/// <summary>
/// Главный класс управления меню и циклом игры.
/// </summary>
public sealed class ConsoleMenu
{
    private readonly ConsoleRenderer _renderer = new();
    private readonly InputReader _input = new();
    private readonly FileGameStorage _gameStorage = new();
    private readonly FileScoreStorage _scoreStorage = new();

    private const string DefaultSaveFile = "save.json";
    private const string DefaultScoresFile = "scores.json";

    /// <summary>
    /// Запускает главное меню приложения.
    /// </summary>
    public void Run()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== Breakthrough (Прорыв) ===");
            Console.WriteLine("1) Новая игра");
            Console.WriteLine("2) Как играть");
            Console.WriteLine("3) Таблица рекордов");
            Console.WriteLine("0) Выход");
            Console.Write("> ");

            var choice = _input.ReadLineTrimmed();

            switch (choice)
            {
                case "1":
                    StartNewGame();
                    break;
                case "2":
                    Console.Clear();
                    _renderer.PrintHelp();
                    Pause();
                    break;
                case "3":
                    ShowScores();
                    break;
                case "0":
                case ":exit":
                    return;
                default:
                    Console.WriteLine("Не понял. Нажми Enter и попробуй снова.");
                    Pause();
                    break;
            }
        }
    }

    /// <summary>
    /// Инициализирует и начинает новую партию.
    /// </summary>
    private void StartNewGame()
    {
        Console.Clear();
        Console.Write("Имя игрока за White (снизу, ходит вверх): ");
        var wName = _input.ReadLineTrimmed();

        Console.Write("Имя игрока за Black (сверху, ходит вниз): ");
        var bName = _input.ReadLineTrimmed();

        var white = new Player(wName, Side.White);
        var black = new Player(bName, Side.Black);

        var rules = new BreakthroughRules();
        var game = new Game(white, black, rules);
        RunGameLoop(game, rules);
    }

    private void ShowScores()
    {
        Console.Clear();

        var board = _scoreStorage.Load(DefaultScoresFile);
        if (board.Entries.Count == 0)
        {
            Console.WriteLine("Рекордов пока нет.");
            Pause();
            return;
        }

        Console.WriteLine("=== Таблица рекордов (меньше ходов — лучше) ===");
        int i = 1;
        foreach (var e in board.Entries.Take(10))
        {
            Console.WriteLine($"{i,2}. {e.WinnerName} ({e.WinnerSide}) победил {e.LoserName} | ходов: {e.PlyCount} | {e.DateUtc:yyyy-MM-dd HH:mm} UTC");
            i++;
        }

        Console.WriteLine();
        Pause();
    }

    /// <summary>
    /// Основной игровой цикл: ввод команд, обработка ходов, рендеринг.
    /// </summary>
    /// <param name="game"></param>
    /// <param name="rules"></param>
    private void RunGameLoop(Game game, IBreakthroughRules rules)
    {
        var current = game;

        // Бесконечный цикл, пока игра не закончится или пользователь не выйдет.
        while (true)
        {
            // 1. Отрисовка текущего состояния доски.
            _renderer.Render(current);
            Console.Write("> ");

            // 2. Чтение ввода пользователя.
            var line = _input.ReadLineTrimmed();

            // Команда немедленного выхода из приложения.
            if (string.Equals(line, ":exit", StringComparison.OrdinalIgnoreCase))
                Environment.Exit(0);

            // Возврат в главное меню (прерывание партии).
            if (string.Equals(line, ":menu", StringComparison.OrdinalIgnoreCase))
                return;

            // Вывод справки.
            if (string.Equals(line, ":help", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine();
                _renderer.PrintHelp();
                Pause();
                continue;
            }

            // Просмотр таблицы рекордов прямо во время игры.
            if (string.Equals(line, ":scores", StringComparison.OrdinalIgnoreCase))
            {
                ShowScores();
                continue;
            }

            // Подсказка доступных ходов для текущего игрока.
            if (string.Equals(line, ":moves", StringComparison.OrdinalIgnoreCase))
            {
                var moves = current.GetAllLegalMovesFor(current.Turn);
                Console.WriteLine();
                Console.WriteLine($"Доступных ходов: {moves.Count}");

                // Выводим только первые 30, чтобы не засорять консоль.
                foreach (var mv in moves.Take(30))
                    Console.WriteLine($"{mv.From} -> {mv.To}");
                if (moves.Count > 30)
                    Console.WriteLine("... (показаны первые 30)");
                Console.WriteLine();
                Pause();
                continue;
            }

            // Команда сохранения игры: :save [имя_файла].
            if (line.StartsWith(":save", StringComparison.OrdinalIgnoreCase))
            {
                var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                var path = parts.Length >= 2 ? parts[1] : DefaultSaveFile;

                try
                {
                    _gameStorage.Save(path, current.ToStateDto());
                    Console.WriteLine($"Сохранено в файл: {path}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка сохранения: {ex.Message}");
                }

                Pause();
                continue;
            }

            // Команда загрузки игры: :load [имя_файла].
            if (line.StartsWith(":load", StringComparison.OrdinalIgnoreCase))
            {
                var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                var path = parts.Length >= 2 ? parts[1] : DefaultSaveFile;

                try
                {
                    var dto = _gameStorage.Load(path);
                    current = Game.FromStateDto(dto, rules);
                    Console.WriteLine($"Загружено из файла: {path}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка загрузки: {ex.Message}");
                }

                Pause();
                continue;
            }

            // 3. Попытка распарсить введенные координаты (например, "a2 a3").
            if (!_input.TryReadMove(line, out var move, out var parseError))
            {
                Console.WriteLine(parseError);
                Pause();
                continue;
            }

            // 4. Попытка применить ход в движке игры.
            // Если ход невалиден (не по правилам), вернется false и текст ошибки.
            if (!current.TryApplyMove(move, out var error, out var winner))
            {
                Console.WriteLine(error);
                Pause();
                continue;
            }

            // 5. Проверка окончания игры.
            // Если winner не null, значит ход привел к победе.
            if (winner is not null)
            {
                _renderer.Render(current); // Финальная отрисовка доски.

                var winnerName = winner == Side.White ? current.White.Name : current.Black.Name;
                var loserName  = winner == Side.White ? current.Black.Name : current.White.Name;

                Console.WriteLine($"Победа! Победил: {winnerName} ({winner})");
                Console.WriteLine($"Ходов (полходов): {current.PlyCount}");

                // Попытка сохранить результат в таблицу рекордов.
                try
                {
                    _scoreStorage.AddResult(DefaultScoresFile, new ScoreEntryDto
                    {
                        WinnerName = winnerName,
                        WinnerSide = winner.Value,
                        LoserName = loserName,
                        PlyCount = current.PlyCount,
                        DateUtc = DateTime.UtcNow
                    });
                    Console.WriteLine("Результат добавлен в таблицу рекордов (scores.json).");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Не удалось записать рекорд: {ex.Message}");
                }

                Pause();
                return; // Выход из цикла игры в главное меню.
            }
        }
    }

    private static void Pause()
    {
        Console.WriteLine("Нажмите Enter...");
        Console.ReadLine();
    }
}
