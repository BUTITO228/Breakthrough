using Breakthrough.ConsoleApp.UI;

namespace Breakthrough.ConsoleApp;

/// <summary>
/// Точка входа консольного приложения.
/// Инициализирует базовые настройки консоли и запускает главное меню игры.
/// </summary>
internal static class Program
{
    /// <summary>
    /// Главный метод приложения (entry point).
    /// </summary>
    /// <param name="args">Аргументы командной строки (в проекте не используются).</param>
    private static void Main(string[] args)
    {
        // Корректный вывод русских символов в большинстве терминалов.
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        var menu = new ConsoleMenu();
        menu.Run();
    }
}
