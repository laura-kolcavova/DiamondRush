
using LightECS;

namespace DiamondRush.MonoGame.Play.Components;

internal sealed record GemPlayBehavior
{
    public Entity GameBoardEntity { get; }

    public GameBoardField? TargetGameBoardField { get; private set; }

    public bool IsAttachedToGameBoardField { get; private set; }

    public bool IsFalling { get; private set; }

    public bool IsVisible { get; private set; } = true;

    public bool IsMatching { get; init; } = false;

    public bool IsCollecting { get; init; } = false;

    public bool IsCollected { get; init; } = false;

    public GemPlayBehavior(
        Entity gameBoardEntity)
    {
        GameBoardEntity = gameBoardEntity;
    }

    public GemPlayBehavior StartFallingToGameBoardField(
        GameBoardField targetGameBoardField)
    {
        return this with
        {
            TargetGameBoardField = targetGameBoardField,
            IsFalling = true,
        };
    }

    public GemPlayBehavior FinishFallingToGameBoardField()
    {
        return this with
        {
            TargetGameBoardField = null,
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
            IsCollecting = false,
            IsCollected = true,
        };
    }
}
