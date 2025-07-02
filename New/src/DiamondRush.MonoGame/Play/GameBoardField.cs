using DiamondRush.MonoGame.Play.Components;

namespace DiamondRush.MonoGame.Play;

internal sealed class GameBoardField
{
    private Gem? _gem;

    public int RowIndex { get; }

    public int ColumnIndex { get; }

    public GameBoardField(
        int rowIndex,
        int columnIndex)
    {
        RowIndex = rowIndex;
        ColumnIndex = columnIndex;
    }

    public Gem Gem => _gem
        ?? throw new InvalidOperationException("No gem is attached to this field.");

    public bool IsEmpty => _gem is null;

    public bool IsOccupied => _gem is not null;

    public void AttachGem(Gem gem)
    {
        _gem = gem;
    }

    public void DetachGem()
    {
        _gem = null;
    }
}
