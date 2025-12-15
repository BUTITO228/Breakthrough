using System.Text.Json;
using Breakthrough.ConsoleApp.Persistence.Models;

namespace Breakthrough.ConsoleApp.Persistence;

/// <summary>
/// Реализация <see cref="IGameStorage"/>, сохраняющая и загружающая состояние игры в JSON-файл.
/// </summary>
public sealed class FileGameStorage : IGameStorage
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true
    };

    /// <summary>
    /// Сохраняет состояние игры в JSON-файл.
    /// Если директория в пути отсутствует, она будет создана.
    /// </summary>
    /// <param name="path">Путь к файлу сохранения (например, save.json).</param>
    /// <param name="state">Сериализуемое состояние игры.</param>
    /// <exception cref="ArgumentException">Если путь пустой или состоит из пробелов.</exception>
    public void Save(string path, GameStateDto state)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Путь к файлу пустой.", nameof(path));

        var json = JsonSerializer.Serialize(state, Options);

        var dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrWhiteSpace(dir))
            Directory.CreateDirectory(dir);

        File.WriteAllText(path, json);
    }

    /// <summary>
    /// Загружает состояние игры из JSON-файла.
    /// Выполняется базовая валидация структуры данных (размер доски 8×8).
    /// </summary>
    /// <param name="path">Путь к файлу сохранения (например, save.json).</param>
    /// <returns>Считанное состояние игры.</returns>
    /// <exception cref="FileNotFoundException">Если файл не найден.</exception>
    /// <exception cref="InvalidDataException">Если файл имеет некорректный формат.</exception>
    public GameStateDto Load(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException("Файл сохранения не найден.", path);

        var json = File.ReadAllText(path);
        var state = JsonSerializer.Deserialize<GameStateDto>(json, Options);

        if (state is null)
            throw new InvalidDataException("Не удалось прочитать сохранение (пустой/битый JSON).");

        if (state.Rows is null || state.Rows.Length != 8 || state.Rows.Any(r => r is null || r.Length != 8))
            throw new InvalidDataException("Некорректный формат доски в сохранении.");

        return state;
    }
}
