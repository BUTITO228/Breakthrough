using System.Text.Json;
using Breakthrough.ConsoleApp.Persistence.Models;

namespace Breakthrough.ConsoleApp.Persistence;

/// <summary>
/// Реализация хранилища рекордов в файле JSON.
/// </summary>
public sealed class FileScoreStorage : IScoreStorage
{
    private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };

    /// <summary>
    /// Загружает таблицу рекордов из файла.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public ScoreboardDto Load(string path)
    {
        if (!File.Exists(path))
            return new ScoreboardDto();

        var json = File.ReadAllText(path);
        var board = JsonSerializer.Deserialize<ScoreboardDto>(json, Options);
        return board ?? new ScoreboardDto();
    }

    /// <summary>
    /// Добавляет новый результат, сортирует таблицу и сохраняет топ лучших.
    /// </summary>
    /// <param name="path">Путь к файлу.</param>
    /// <param name="entry">Запись о результате игры.</param>
    /// <param name="keepTop">Сколько записей хранить (по умолчанию 20).</param>
    public void AddResult(string path, ScoreEntryDto entry, int keepTop = 20)
    {
        var board = Load(path);
        board.Entries.Add(entry);

        // Логика сортировки:
        // 1. Приоритет - меньше ходов (PlyCount).
        // 2. Если ходов поровну - новее дата (DateUtc).
        board.Entries = board.Entries
            .OrderBy(e => e.PlyCount)
            .ThenByDescending(e => e.DateUtc)
            .Take(Math.Max(1, keepTop))
            .ToList();

        var json = JsonSerializer.Serialize(board, Options);

        // Создаем директорию, если её нет.
        var dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrWhiteSpace(dir))
            Directory.CreateDirectory(dir);

        File.WriteAllText(path, json);
    }
}
