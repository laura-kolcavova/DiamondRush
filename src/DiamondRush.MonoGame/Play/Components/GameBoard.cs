namespace DiamondRush.MonoGame.Play.Components;

internal sealed record GameBoard
{
    public required int Rows { get; init; }

    public required int Columns { get; init; }
}
