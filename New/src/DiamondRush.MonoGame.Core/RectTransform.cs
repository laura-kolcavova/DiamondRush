
using Microsoft.Xna.Framework;

namespace DiamondRush.MonoGame.Core;

public sealed record RectTransform
{
    public Vector2 Position { get; init; } = Vector2.Zero;

    public float Width { get; init; } = 0;

    public float Height { get; init; } = 0;

    public float Left => Position.X;

    public float Right => Position.X + Width;

    public float Top => Position.Y;

    public float Bottom => Position.Y + Height;

    public bool IsInside(RectTransform rectTransform)
    {
        return rectTransform.Left <= Left
            && rectTransform.Right >= Right
            && rectTransform.Top <= Top
            && rectTransform.Bottom >= Bottom;
    }

    public RectTransform UpdatePosition(
        Vector2 newPosition)
    {
        return this with
        {
            Position = newPosition,
        };
    }
}
