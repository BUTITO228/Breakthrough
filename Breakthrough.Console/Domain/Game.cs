using Breakthrough.ConsoleApp.Common;
using Breakthrough.ConsoleApp.Domain.Rules;
using Breakthrough.ConsoleApp.Persistence.Models;

namespace Breakthrough.ConsoleApp.Domain;

/// <summary>
/// Основной класс игры, управляющий состоянием доски, игроками и очередностью ходов.
/// </summary>
public sealed class Game
{
    /// <summary>
    /// Игровое поле.
    /// </summary>
    public Board Board { get; } = new();

    /// <summary>
    /// Игрок за белых.
    /// </summary>
    public Player White { get; }

    /// <summary>
    /// Игрок за черных.
    /// </summary>
    public Player Black { get; }

    /// <summary>
    /// Текущая сторона, которая должна сделать ход.
    /// </summary>
    public Side Turn { get; private set; } = Side.White;

    /// <summary>
    /// Правила игры.
    /// </summary>
    public IBreakthroughRules Rules { get; }

    /// <summary>
    /// Счетчик ходов (ply) с начала игры.
    /// </summary>
    public int PlyCount { get; private set; } = 0;

    /// <summary>
    /// Создает новую игру.
    /// </summary>
    /// <param name="white">Игрок за белых.</param>
    /// <param name="black">Игрок за черных.</param>
    /// <param name="rules">Реализация правил.</param>
    /// <param name="turn">Чей сейчас ход.</param>
    /// <param name="setup">Нужно ли расставить фигуры в начальную позицию.</param>
    /// <param name="plyCount">Текущий счетчик ходов.</param>
    public Game(Player white, Player black, IBreakthroughRules rules, Side turn = Side.White, bool setup = true, int plyCount = 0)
    {
        White = white;
        Black = black;
        Rules = rules;
        Turn = turn;
        PlyCount = plyCount;

        if (setup)
            Board.ResetInitialSetup();
    }

    /// <summary>
    /// Возвращает объект текущего игрока.
    /// </summary>
    public Player CurrentPlayer => Turn == Side.White ? White : Black;

    /// <summary>
    /// Пытается применить ход. Проверяет валидность хода согласно правилам.
    /// </summary>
    /// <param name="move">Ход (откуда -> куда).</param>
    /// <param name="error">Текст ошибки, если ход недопустим.</param>
    /// <param name="winner">Возвращает победителя, если ход привел к победе.</param>
    /// <returns>True, если ход успешно сделан; иначе False.</returns>
    /// <summary>
    /// Пытается применить ход. Проверяет валидность хода согласно правилам.
    /// </summary>
    public bool TryApplyMove(Move move, out string error, out Side? winner)
    {
        error = string.Empty;
        winner = null;

        var from = move.From;
        var to = move.To;

        if (!from.IsInside() || !to.IsInside())
        {
            error = "Координаты вне доски.";
            return false;
        }

        var piece = Board.GetAt(from);
        if (piece is null)
        {
            error = "В исходной клетке нет фишки.";
            return false;
        }

        if (piece.Side != Turn)
        {
            error = "Это не ваша фишка.";
            return false;
        }

        int dir = Rules.ForwardDirection(Turn);
        int dr = to.Row - from.Row;
        int dc = to.Col - from.Col;
        var target = Board.GetAt(to);

        // Проверяем направление движения (строго вперед на 1 ряд)
        if (dr != dir)
        {
            error = "Фишки могут ходить только вперед.";
            return false;
        }

        // Проверяем смещение по горизонтали (0 = прямо, 1 = диагональ, >1 = нельзя)
        if (Math.Abs(dc) > 1)
        {
            error = "Фишка ходит только на соседние клетки (прямо или по диагонали).";
            return false;
        }

        // Проверка: целевая клетка не должна быть занята СВОЕЙ фигурой
        if (target is not null && target.Side == Turn)
        {
            error = "Клетка занята вашей фигурой.";
            return false;
        }

        // Специфика хода ПРЯМО: клетка должна быть пустой
        if (dc == 0 && target is not null)
        {
            error = "Вперёд можно ходить только в пустую клетку. Для взятия используйте диагональ.";
            return false;
        }

        // Если мы здесь, значит ход валиден:
        // 1. Либо ход прямо в пустую.
        // 2. Либо ход по диагонали в пустую.
        // 3. Либо ход по диагонали со взятием (target != null, но мы проверили выше, что он не свой).

        DoMove(from, to);
        PlyCount++;
        winner = CheckWinnerAfterMove(to, Turn);
        EndTurnIfNotOver(winner);
        return true;
    }


    /// <summary>
    /// Проверяет, есть ли у стороны хотя бы один легальный ход.
    /// Используется для определения пата/проигрыша.
    /// </summary>
    /// <summary>
    /// Проверяет, есть ли у стороны хотя бы один легальный ход.
    /// </summary>
    public bool HasAnyLegalMove(Side side)
    {
        int dir = Rules.ForwardDirection(side);

        foreach (var (pos, piece) in Board.EnumeratePieces())
        {
            if (piece.Side != side) continue;

            // 1. Проверка хода прямо (только если пусто)
            var forward = new Position(pos.Row + dir, pos.Col);
            if (forward.IsInside() && Board.GetAt(forward) is null)
                return true;

            // 2. Проверка диагонали влево (если пусто ИЛИ враг)
            var diagL = new Position(pos.Row + dir, pos.Col - 1);
            if (diagL.IsInside())
            {
                var target = Board.GetAt(diagL);
                // Можно ходить, если там нет моей фигуры
                if (target is null || target.Side != side)
                    return true;
            }

            // 3. Проверка диагонали вправо (если пусто ИЛИ враг)
            var diagR = new Position(pos.Row + dir, pos.Col + 1);
            if (diagR.IsInside())
            {
                var target = Board.GetAt(diagR);
                // Можно ходить, если там нет моей фигуры
                if (target is null || target.Side != side)
                    return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Возвращает список всех возможных ходов для указанной стороны.
    /// Используется для подсказок (:moves) и ботов.
    /// </summary>
    /// <summary>
    /// Возвращает список всех возможных ходов для указанной стороны.
    /// </summary>
    public List<Move> GetAllLegalMovesFor(Side side)
    {
        var moves = new List<Move>();
        int dir = Rules.ForwardDirection(side);

        foreach (var (pos, piece) in Board.EnumeratePieces())
        {
            if (piece.Side != side) continue;

            // Ход прямо (только если пусто)
            var forward = new Position(pos.Row + dir, pos.Col);
            if (forward.IsInside() && Board.GetAt(forward) is null)
                moves.Add(new Move(pos, forward));

            // Диагональ влево (пусто или враг)
            var diagL = new Position(pos.Row + dir, pos.Col - 1);
            if (diagL.IsInside())
            {
                var target = Board.GetAt(diagL);
                if (target is null || target.Side != side)
                    moves.Add(new Move(pos, diagL));
            }

            // Диагональ вправо (пусто или враг)
            var diagR = new Position(pos.Row + dir, pos.Col + 1);
            if (diagR.IsInside())
            {
                var target = Board.GetAt(diagR);
                if (target is null || target.Side != side)
                    moves.Add(new Move(pos, diagR));
            }
        }

        return moves;
    }

    /// <summary>
    /// Преобразует текущее состояние игры в DTO для сохранения.
    /// </summary>
    /// <returns></returns>
    public GameStateDto ToStateDto()
    {
        var rows = new string[8];

        for (int r = 0; r < 8; r++)
        {
            var chars = new char[8];
            for (int c = 0; c < 8; c++)
            {
                var piece = Board.GetAt(new Position(r, c));
                chars[c] = piece is null ? '.' : (piece.Side == Side.White ? 'W' : 'B');
            }
            rows[r] = new string(chars);
        }

        return new GameStateDto
        {
            WhiteName = White.Name,
            BlackName = Black.Name,
            Turn = Turn,
            PlyCount = PlyCount,
            Rows = rows
        };
    }

    /// <summary>
    /// Восстанавливает игру из DTO.
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="rules"></param>
    /// <returns></returns>
    /// <exception cref="InvalidDataException"></exception>
    public static Game FromStateDto(GameStateDto dto, IBreakthroughRules rules)
    {
        var white = new Player(dto.WhiteName, Side.White);
        var black = new Player(dto.BlackName, Side.Black);

        var game = new Game(white, black, rules, dto.Turn, setup: false, plyCount: dto.PlyCount);

        for (int r = 0; r < 8; r++)
        {
            for (int c = 0; c < 8; c++)
            {
                char ch = dto.Rows[r][c];
                var pos = new Position(r, c);

                game.Board.SetAt(pos, ch switch
                {
                    'W' => new Pawn(Side.White),
                    'B' => new Pawn(Side.Black),
                    '.' => null,
                    _ => throw new InvalidDataException($"Некорректный символ '{ch}' в сохранении.")
                });
            }
        }

        return game;
    }

    /// <summary>
    /// Выполняет перемещение фишки на доске (без проверок).
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    private void DoMove(Position from, Position to)
    {
        var moving = Board.GetAt(from)!;
        Board.SetAt(from, null);
        Board.SetAt(to, moving);
    }

    /// <summary>
    /// Проверяет условия победы после сделанного хода.
    /// </summary>
    /// <param name="lastTo"></param>
    /// <param name="mover"></param>
    /// <returns></returns>
    private Side? CheckWinnerAfterMove(Position lastTo, Side mover)
    {
        // 1. Фишка дошла до противоположного края.
        if (Rules.IsWinningRow(mover, lastTo.Row))
            return mover;

        // 2. У противника не осталось фигур.
        bool opponentHasPieces = Board.EnumeratePieces().Any(x => x.Piece.Side == mover.Opponent());
        if (!opponentHasPieces) return mover;

        // 3. У противника нет доступных ходов (блокада).
        if (!HasAnyLegalMove(mover.Opponent()))
            return mover;

        return null;
    }

    /// <summary>
    /// Передает ход другому игроку, если игра не окончена.
    /// </summary>
    /// <param name="winner"></param>
    private void EndTurnIfNotOver(Side? winner)
    {
        if (winner is null)
            Turn = Turn.Opponent();
    }
}
