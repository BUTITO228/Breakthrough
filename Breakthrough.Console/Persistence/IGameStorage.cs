using Breakthrough.ConsoleApp.Persistence.Models;

namespace Breakthrough.ConsoleApp.Persistence;

/// <summary>
/// Интерфейс для сохранения и загрузки игры.
/// </summary>
public interface IGameStorage
{
    void Save(string path, GameStateDto state);
    GameStateDto Load(string path);
}
