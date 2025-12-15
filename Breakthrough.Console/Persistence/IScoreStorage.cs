using Breakthrough.ConsoleApp.Persistence.Models;

namespace Breakthrough.ConsoleApp.Persistence;

/// <summary>
/// Интерфейс хранилища таблицы рекордов (top-scores).
/// Отвечает за загрузку таблицы и добавление нового результата с сохранением.
/// </summary>
public interface IScoreStorage
{
    /// <summary>
    /// Загружает таблицу рекордов из указанного источника.
    /// Если таблица отсутствует, реализация может вернуть пустую таблицу.
    /// </summary>
    /// <param name="path">Путь к файлу/ресурсу с таблицей рекордов.</param>
    /// <returns>Таблица рекордов (возможно пустая).</returns>
    ScoreboardDto Load(string path);

    /// <summary>
    /// Добавляет результат в таблицу рекордов и сохраняет её.
    /// Реализация должна отсортировать записи и оставить только <paramref name="keepTop"/> лучших.
    /// </summary>
    /// <param name="path">Путь к файлу/ресурсу с таблицей рекордов.</param>
    /// <param name="entry">Добавляемая запись результата.</param>
    /// <param name="keepTop">Сколько лучших результатов хранить (по умолчанию 20).</param>
    void AddResult(string path, ScoreEntryDto entry, int keepTop = 20);
}
