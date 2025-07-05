
using Microsoft.Xna.Framework;

namespace DiamondRush.MonoGame.Play.Components;

internal sealed record GemPlayBehavior
{
    public GameBoardField? TargetGameBoardField { get; private set; }

    public Vector2? TargetGameBoardFieldPosition { get; private set; }

    public bool IsFalling { get; private set; }

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
}
