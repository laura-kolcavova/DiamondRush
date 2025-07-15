namespace DiamondRush.MonoGame.Play.Components;

internal sealed record GemPlayBehavior
{
    public int CurrentRowIndex { get; private set; } = -1;

    public int CurrentColumnIndex { get; private set; } = -1;

    public int OriginalRowIndex { get; private set; } = -1;

    public int OriginalColumnIndex { get; private set; } = -1;

    public int TargetRowIndex { get; private set; } = -1;

    public int TargetColumnIndex { get; private set; } = -1;

    public bool IsFalling { get; private set; } = false;

    public bool IsVisible { get; private set; } = true;

    public bool IsMatching { get; init; } = false;

    public bool IsCollecting { get; init; } = false;

    public bool IsCollected { get; init; } = false;

    public bool CollectAnimationEnabled { get; init; } = false;

    public float CollectAnimationProgress { get; init; } = 0f;

    public bool IsSwapping { get; private set; } = false;

    public bool IsSwapped { get; private set; } = false;

    public bool IsSwappingBack { get; private set; } = false;

    public GemPlayBehavior StartFalling(
        int targetRowIndex,
        int targetColumnIndex)
    {
        return this with
        {
            IsFalling = true,
            TargetRowIndex = targetRowIndex,
            TargetColumnIndex = targetColumnIndex,
        };
    }

    public GemPlayBehavior FinishFalling()
    {
        return this with
        {
            IsFalling = false,
            OriginalRowIndex = TargetRowIndex,
            OriginalColumnIndex = TargetColumnIndex,
            CurrentRowIndex = TargetRowIndex,
            CurrentColumnIndex = TargetColumnIndex,
            TargetRowIndex = -1,
            TargetColumnIndex = -1,
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
            CollectAnimationEnabled = true,
        };
    }

    public GemPlayBehavior FinishCollecting()
    {
        return this with
        {
            IsCollecting = false,
            IsCollected = true,
            CollectAnimationEnabled = false,
            CurrentRowIndex = -1,
            CurrentColumnIndex = -1,
        };
    }

    public GemPlayBehavior UpdateCollectAnimationProgress(
        float progress)
    {
        return this with
        {
            CollectAnimationProgress = Math.Clamp(
                progress,
                0f,
                1f),
        };
    }

    public GemPlayBehavior StartSwapping(
        int targetRowIndex,
        int targetColumnIndex)
    {
        return this with
        {
            IsSwapping = true,
            TargetRowIndex = targetRowIndex,
            TargetColumnIndex = targetColumnIndex,
        };
    }

    public GemPlayBehavior FinishSwapping()
    {
        return this with
        {
            IsSwapping = false,
            IsSwapped = true,
            CurrentRowIndex = TargetRowIndex,
            CurrentColumnIndex = TargetColumnIndex,
            TargetRowIndex = -1,
            TargetColumnIndex = -1,
        };
    }

    public GemPlayBehavior ConfirmSwappedPosition()
    {
        return this with
        {
            IsSwapping = false,
            IsSwapped = false,
            OriginalRowIndex = CurrentRowIndex,
            OriginalColumnIndex = CurrentColumnIndex,
        };
    }

    public GemPlayBehavior StartSwappingBack()
    {
        return this with
        {
            IsSwappingBack = true,
        };
    }

    public GemPlayBehavior FinishSwappingBack()
    {
        return this with
        {
            IsSwappingBack = false,
            IsSwapped = false,
            CurrentRowIndex = OriginalRowIndex,
            CurrentColumnIndex = OriginalColumnIndex,
        };
    }
}
