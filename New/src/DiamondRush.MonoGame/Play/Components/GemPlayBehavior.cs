namespace DiamondRush.MonoGame.Play.Components;

internal sealed record GemPlayBehavior
{
    public int TargetRowIndex { get; private set; } = -1;

    public int TargetColumnIndex { get; private set; } = -1;

    public int AttachedToRowIndex { get; private set; } = -1;

    public int AttachedColumnIndex { get; private set; } = -1;

    public bool IsFalling { get; private set; } = false;

    public bool IsVisible { get; private set; } = true;

    public bool IsMatching { get; init; } = false;

    public bool IsCollecting { get; init; } = false;

    public bool IsCollected { get; init; } = false;

    public GemPlayBehavior StartFallingToGameBoardField(
        int rowIndex,
        int columnIndex)
    {
        return this with
        {
            TargetRowIndex = rowIndex,
            TargetColumnIndex = columnIndex,
            IsFalling = true,
        };
    }

    public GemPlayBehavior FinishFallingToGameBoardField()
    {
        return this with
        {
            AttachedToRowIndex = TargetRowIndex,
            AttachedColumnIndex = TargetColumnIndex,
            TargetRowIndex = -1,
            TargetColumnIndex = -1,
            IsFalling = false,
        };
    }

    public GemPlayBehavior SetVisibility(
        bool isVisible)
    {
        return this with
        {
            IsVisible = isVisible,
        };
    }

    public GemPlayBehavior MarkAsMatching()
    {
        return this with
        {
            IsMatching = true,
        };
    }

    public GemPlayBehavior StartCollecting()
    {
        return this with
        {
            IsCollecting = true,
        };
    }

    public GemPlayBehavior FinishCollecting()
    {
        return this with
        {
            AttachedToRowIndex = -1,
            AttachedColumnIndex = -1,
            IsCollecting = false,
            IsCollected = true,
        };
    }
}
