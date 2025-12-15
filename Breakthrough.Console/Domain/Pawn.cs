using Breakthrough.ConsoleApp.Common;

namespace Breakthrough.ConsoleApp.Domain;

/// <summary>
/// Пешка — единственная фигура в игре "Прорыв".
/// </summary>
public sealed class Pawn : Piece
{
    public Pawn(Side side) : base(side) { }
    public override char Symbol => Side == Side.White ? 'W' : 'B';
}
