using LightECS;

namespace DiamondRush.MonoGame.Play;

internal sealed class GameBoardField
{
    private Entity? _gemEntity;

    public int RowIndex { get; }

    public int ColumnIndex { get; }

    public GameBoardField(
        int rowIndex,
        int columnIndex)
    {
        RowIndex = rowIndex;
        ColumnIndex = columnIndex;
    }

    public Entity GemEntity => _gemEntity
        ?? throw new InvalidOperationException("No gem is attached to this field.");

    public bool IsEmpty => _gemEntity is null;

    public bool IsOccupied => _gemEntity is not null;

    public void AttachGem(Entity gemEntity)
    {
        _gemEntity = gemEntity;
    }

    public void DetachGem()
    {
        _gemEntity = null;
    }
}
