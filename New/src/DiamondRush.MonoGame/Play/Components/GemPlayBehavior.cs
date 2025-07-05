
using LightECS;
using Microsoft.Xna.Framework;

namespace DiamondRush.MonoGame.Play.Components;

internal sealed record GemPlayBehavior
{
    public Entity GameBoardEntity { get; }

    public GameBoardField? TargetGameBoardField { get; private set; }

    public Vector2? TargetGameBoardFieldPosition { get; private set; }

    public bool IsFalling { get; private set; }

    public bool IsVisible { get; private set; } = true;

    public GemPlayBehavior(
        Entity gameBoardEntity)
    {
        GameBoardEntity = gameBoardEntity;
    }

    public GemPlayBehavior StartFallingToGameBoardField(
        GameBoardField targetGameBoardField,
        Vector2 targetGameBoardFieldPosition)
    {
        return this with
        {
            TargetGameBoardField = targetGameBoardField,
            TargetGameBoardFieldPosition = targetGameBoardFieldPosition,
            IsFalling = true,
        };
    }

    public GemPlayBehavior FinishFallingToGameBoardField()
    {
        return this with
        {
            TargetGameBoardField = null,
            TargetGameBoardFieldPosition = null,
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
}
